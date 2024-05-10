using Microsoft.AspNetCore.Mvc;

namespace Lexi.Core.Api.wwwroot.AiVoices
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
