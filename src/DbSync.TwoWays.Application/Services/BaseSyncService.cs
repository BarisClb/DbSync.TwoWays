using Confluent.Kafka;
using DbSync.TwoWays.Application.DbServices;

namespace DbSync.TwoWays.Application.Services;

public abstract class BaseSyncConsumer
{
    protected readonly IMsSqlDbService MsSql;
    protected readonly IPostgreSqlDbService Pg;

    protected BaseSyncConsumer(
        IMsSqlDbService msSql,
        IPostgreSqlDbService pg,
        string bootstrapServers,
        string groupId,
        string topic)
    {
        MsSql = msSql;
        Pg = pg;
        Topic = topic;

        KafkaConfig = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true
        };
    }

    protected ConsumerConfig KafkaConfig { get; }
    protected string Topic { get; }

    private CancellationTokenSource? _cts;
    private Task? _task;

    public bool IsRunning => _task is not null && !_task.IsCompleted;

    public void Start()
    {
        if (IsRunning) return;
        _cts = new CancellationTokenSource();
        _task = Task.Run(() => ConsumeLoopAsync(_cts.Token));
    }

    public async Task StopAsync()
    {
        if (!IsRunning || _cts is null || _task is null) return;
        _cts.Cancel();
        await _task;
        _cts = null;
        _task = null;
    }

    private async Task ConsumeLoopAsync(CancellationToken ct)
    {
        using var consumer = new ConsumerBuilder<string, string>(KafkaConfig).Build();
        consumer.Subscribe(Topic);

        Console.WriteLine($"{GetType().Name} started. Topic={Topic}");

        try
        {
            while (!ct.IsCancellationRequested)
            {
                var cr = consumer.Consume(ct);
                if (cr?.Message?.Value is null)
                    continue;

                await OnMessageAsync(cr.Message.Value, ct);
            }
        }
        catch (OperationCanceledException)
        {
            // expected on shutdown
        }
        finally
        {
            consumer.Close();
            Console.WriteLine($"{GetType().Name} stopped.");
        }
    }

    protected virtual Task OnMessageAsync(string value, CancellationToken ct)
        => Task.CompletedTask;
}