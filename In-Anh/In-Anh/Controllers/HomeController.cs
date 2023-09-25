using In_Anh.Models;
using In_Anh.RabitMQ;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace In_Anh.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IConfiguration config, IImageMgDatabase setting, IRabitMQProducer rabitMQProducer) : base(config, setting, rabitMQProducer)
        {
        }

        public IActionResult Index()
        {
			int worker = 0;
			int io = 0;
			ThreadPool.GetAvailableThreads(out worker, out io);

			Console.WriteLine("Thread pool threads available at startup: ");
			Console.WriteLine("   Worker threads: {0:N0}", worker);
			Console.WriteLine("   Asynchronous I/O threads: {0:N0}", io);
			Languages lang = new Languages().LanguageVN();
            ViewBag.Language = lang;
            ViewBag.Current = "Home";
            ViewBag.IsLogin = isLogin();
            if (isLogin())
            {
                var token = Request.Cookies["userToken"];


                var validuser = GetUserValid(token);
                ViewBag.UserLogin = validuser;

                
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}