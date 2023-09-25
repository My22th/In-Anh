using In_Anh.Models;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Printing;

namespace In_Anh.Controllers
{
    public class SharedController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult RenderTemplate()
        {
            return View();
        }
        public IActionResult PriceTable()
        {
            Languages lang = new Languages().LanguageVN();
            ViewBag.Language = lang;
            ViewBag.Current ="PriceTable";
            ViewBag.IsLogin = false;
            return View();
        }
    }
}
