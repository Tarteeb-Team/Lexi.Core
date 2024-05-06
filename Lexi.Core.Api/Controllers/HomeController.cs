using Microsoft.AspNetCore.Mvc;

namespace Lexi.Core.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : Controller
    {
        [HttpGet]
        public string GetHome() =>
            "Hi, home!";
    }
}
