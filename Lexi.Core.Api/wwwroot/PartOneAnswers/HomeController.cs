using Microsoft.AspNetCore.Mvc;

namespace Lexi.Core.Api.wwwroot.outputWavs
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
