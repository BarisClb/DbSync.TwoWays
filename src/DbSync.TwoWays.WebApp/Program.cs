using DbSync.TwoWays.Application;
using DbSync.TwoWays.Application.Models;
using DbSync.TwoWays.Application.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.RegisterApplicationServices();

builder.Services.Configure<KafkaConsumerSettings>(builder.Configuration.GetSection("Kafka"));
builder.Services.RegisterConsumers(builder.Configuration);


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

app.MapPost("/consumer/1/start", (SyncService1 s) => { s.Start(); return Results.Ok("started 1"); });
app.MapPost("/consumer/1/stop", async (SyncService1 s) => { await s.StopAsync(); return Results.Ok("stopped 1"); });
app.MapGet("/consumer/1/status", (SyncService1 s) => Results.Ok(new { s.IsRunning }));

app.MapPost("/consumer/2/start", (SyncService2 s) => { s.Start(); return Results.Ok("started 2"); });
app.MapPost("/consumer/2/stop", async (SyncService2 s) => { await s.StopAsync(); return Results.Ok("stopped 2"); });
app.MapGet("/consumer/2/status", (SyncService2 s) => Results.Ok(new { s.IsRunning }));

app.MapPost("/consumer/3/start", (SyncService3 s) => { s.Start(); return Results.Ok("started 3"); });
app.MapPost("/consumer/3/stop", async (SyncService3 s) => { await s.StopAsync(); return Results.Ok("stopped 3"); });
app.MapGet("/consumer/3/status", (SyncService3 s) => Results.Ok(new { s.IsRunning }));

app.MapPost("/consumer/4/start", (SyncService4 s) => { s.Start(); return Results.Ok("started 4"); });
app.MapPost("/consumer/4/stop", async (SyncService4 s) => { await s.StopAsync(); return Results.Ok("stopped 4"); });
app.MapGet("/consumer/4/status", (SyncService4 s) => Results.Ok(new { s.IsRunning }));

app.Run();
