using Microsoft.AspNetCore.Mvc;

namespace Fotobox.Controllers
{
  public class HomeController : Controller
  {
    public IActionResult Index()
    {
      return this.View();
    }
  }
}
