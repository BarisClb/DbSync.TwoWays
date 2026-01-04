using Microsoft.AspNetCore.Mvc;

namespace DbSync.TwoWays.WebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet(Name = "Test")]
        public async Task<IActionResult> Test()
        {
            return Ok();
        }
    }
}
