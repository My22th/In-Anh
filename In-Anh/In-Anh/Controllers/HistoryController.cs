using In_Anh.Models;
using In_Anh.RabitMQ;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Linq;

namespace In_Anh.Controllers
{
    public class HistoryController : BaseController
    {
        private static readonly int pageSize = 2;

        public HistoryController(IConfiguration config, IImageMgDatabase setting, IRabitMQProducer rabitMQProducer) : base(config, setting, rabitMQProducer)
        {
        }


        // GET: HistoryController
        public ActionResult Index()
        {
            Languages lang = new Languages().LanguageVN();
            ViewBag.Language = lang;
            ViewBag.Page = 0;
            ViewBag.PageSize = pageSize;
            ViewBag.IsLogin = false;
            ViewBag.Current = "History";
            var lstId = GetOrderIdsUserCookies();
            var cdnpath = _config["Cdn:UrlCdn"];
            ViewBag.Cdn = cdnpath;
            var phone = GetPhoneUserCookies();
            var orderLst = new List<OrderDetail>() { };
            if (string.IsNullOrEmpty(phone) || lstId == null || lstId.Length ==0)
            {
                return View(orderLst);
            }
            var orderqrall = _ordersCollection?.AsQueryable()?.Where(x => x.Phone == phone).FirstOrDefault()?.ListDetail;
            var orderqr = new List<OrderDetail>();
           if(orderqrall != null)
            {
                foreach (var item in orderqrall)
                {
                    if (lstId.Contains(item.OrderId))
                    {
                        orderqr.Add(item);
                    }
                }
            }
                
            
            var data = orderqr?.OrderByDescending(y=>y.DayOrder).Take(pageSize).ToList();

            
            if (data ==null || !data.Any())
            {
                return View(orderLst);
            }
            foreach (var item in data)
            {
                item.Images = _imagesCollection.FindAsync(x => x.Id == item.OrderId).Result.ToList().GroupBy(x => x.Type).Select(x => new ImageModel() { Type = x.Key, Price = x.FirstOrDefault()?.Price, OrginUrl = x.ToList().SelectMany(y => y.OrginUrl).ToList() }).ToList();
            }
            return View(data);
        }
        public async Task<ActionResult> GetData(int page)
        {
            Languages lang = new Languages().LanguageVN();
            ViewBag.Language = lang;
            ViewBag.IsLogin = false;
            var cdnpath = _config["Cdn:UrlCdn"];
            ViewBag.Cdn = cdnpath;
            var lstId = GetOrderIdsUserCookies();
            var phone = GetPhoneUserCookies();
            var orderLst = new List<OrderDetail>() { };
            if (string.IsNullOrEmpty(phone) || lstId == null || lstId.Length == 0)
            {
                return new JsonResult(new
                {
                    Code = 201,
                    Data = new { },
                    Message = "File Not Valid"
                });
            }

            var orderqrall = _ordersCollection?.AsQueryable()?.Where(x => x.Phone == phone).FirstOrDefault()?.ListDetail;
            var orderqr = new List<OrderDetail>();
           if(orderqrall != null)
            {
                foreach (var item in orderqrall)
                {
                    if (lstId.Contains(item.OrderId))
                    {
                        orderqr.Add(item);
                    }
                }
            }
           
            

            var data = orderqr?.OrderByDescending(y => y.DayOrder).Skip(page * pageSize).Take(pageSize).ToList();


            if (data == null || !data.Any())
            {
                return new JsonResult(new
                {
                    Code = 201,
                    Data = new { },
                    Message = "File Not Valid"
                });
            }
            foreach (var item in data)
            {
                item.Images = _imagesCollection.FindAsync(x => x.Id == item.OrderId).Result.ToList().GroupBy(x => x.Type).Select(x => new ImageModel() { Type = x.Key, Price = x.FirstOrDefault()?.Price, OrginUrl = x.ToList().SelectMany(y => y.OrginUrl).ToList() }).ToList();
            }
            var partialViewHtml = await this.RenderViewAsync("_RenderItem", data, true);
            return new JsonResult(new
            {
                Code = 200,
                Data = new { html=partialViewHtml },
                Message = ""
            });

        }
        // GET: HistoryController/Details/5
        [HttpGet]
        public async Task<ActionResult> RemoveOrderAsync(string phone, string orderID)
        {
            var filter = Builders<OrderModel>.Filter.And(Builders<OrderModel>.Filter.Eq(x => x.Phone, phone), Builders<OrderModel>.Filter.Where(x => x.ListDetail.Any(y => y.OrderId == orderID)));
            var a = _ordersCollection.Find(filter).FirstOrDefault();

            var oldData = _ordersCollection.FindAsync(filter).Result.FirstOrDefault();
            if (oldData != null)
            {
                var dataDetails = oldData.ListDetail.ToList();
                var dataDetail = dataDetails.Find(x => x.OrderId == orderID);
                if (dataDetail != null)
                {
                    dataDetails.Remove(dataDetails.Find(x => x.OrderId == orderID));
                    var update = Builders<OrderModel>.Update
                     .Set(x => x.ListDetail, dataDetails);
                    var data = await _ordersCollection.FindOneAndUpdateAsync(filter, update);
                }
            }
            return Ok();
        }
    }
}
