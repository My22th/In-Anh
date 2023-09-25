using ImageMagick;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using System;
using Newtonsoft.Json;
using MongoDB.Driver;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using In_Anh.Models;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;
using AngleSharp.Media;
using Image = SixLabors.ImageSharp.Image;
using Point = SixLabors.ImageSharp.Point;
using Color = SixLabors.ImageSharp.Color;
using System.Diagnostics;
using static MongoDB.Driver.WriteConcern;
using static In_Anh.Models.OrderModel;
using In_Anh.RabitMQ;
using In_Anh.Models.RabitMQModel;
using RabbitMQ.Client;
using System.Net.Sockets;
using System.Net;
using System.Text;
using AngleSharp.Dom;
using Microsoft.AspNetCore.Http.Extensions;
using System.Numerics;

namespace In_Anh.Controllers
{
	public class ImageController : BaseController
	{
		public ImageController(IConfiguration config, IImageMgDatabase setting, IRabitMQProducer rabitMQProducer) : base(config, setting, rabitMQProducer)
		{
		}


		// GET: ImageController
		public ActionResult Index()
		{
			return View();
		}

		// GET: ImageController/Details/5
		public ActionResult Details(int id)
		{
			return View();
		}




		// POST: ImageController/Create
		[HttpPost]
        [DisableRequestSizeLimit]
        public async Task<ActionResult> CreateAsync(IFormCollection data)
		{
			try
			{
				if(data == null) {
                    return new JsonResult(new
                    {
                        Code = 400,
                        Data = new { },
                        Message = "Nodata"
                    });
                }
				var UserPhone = string.IsNullOrWhiteSpace(GetPhoneUserCookies().ToString()) ? "spam" : GetPhoneUserCookies().ToString();
				var localFile = _config["Cdn:LocalPath"];
				var year = DateTime.Now.Year;
				var month = DateTime.Now.Month;
				var day = DateTime.Now.Day;
				var type = 0;
				int.TryParse(data?["type"].ToString(), out type);
				string nameType = Enum.GetName(typeof(ImageType), type) ?? "1x1";
				var id = GetOrderIdUserCookies();
				var path = UserPhone + "\\" + year + "-" + month + "-" + day + "\\" + id + "\\" + nameType + "\\";
				
				var filePath = localFile + path;
				
				ConnectionFactory factory = new ConnectionFactory();
				factory.Uri = new Uri("amqp://admin:admin@jinnie.shop/%2f");
				ConnectionFactory.DefaultAddressFamily = AddressFamily.InterNetwork;
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
				//Create the RabbitMQ connection using connection factory details as i mentioned above

				IConnection connection = factory.CreateConnection();
				IModel channel = connection.CreateModel();
				Thread.Sleep(1000);
				data.Files.AsParallel().ForAll((file) =>
				{
					if (file.Length > 0)
					{
						using var fileStream = file.OpenReadStream();
						byte[] bytes = new byte[file.Length];
						fileStream.Read(bytes);
						//fileStream.DisposeAsync();

						var json = JsonConvert.SerializeObject(new RabitMQSendData()
						{
							Type = (ImageType) type,
							OrderID = id,
							File = bytes,
							FileName = file.FileName.Replace(".","") + Guid.NewGuid() + ".jpg",
							Path = filePath,
							CDNPath = path,
							TypeActive = (Active)type
						});
						var body = Encoding.UTF8.GetBytes(json);
						_rabitMQProducer.SendProductMessage(connection, channel, body);
					}
				});
				
				return new JsonResult(new
				{
					Code = 200,
					Data = new { },
					Message = "Success"
				});

			}
			catch (Exception e)
			{
				var a = e.Message;
				throw;
			}



		}

		[HttpGet]
		public async Task<ActionResult> CreareOrderID(string orderID, string phone, string address, string name, string note, int total)
		{
			try
			{
				var userGetOrder = _ordersCollection.FindAsync(x => x.Phone == phone).Result.FirstOrDefault();
				var port = Dns.GetHostEntry(Dns.GetHostName())
   .AddressList
   .First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
   .ToString();
				var detail = new OrderDetail()
				{
					OrderId = orderID,
					Address = address,
					UserEmail = "",
					Name = name,
					Note = note,
					IsActive = false,
					Phone = phone,
					DayOrder = new DateTime(DateTime.Now.Ticks),
					Active = Active.Inactive,
					Total = total,
					Port = port
				};
				if (userGetOrder == null)
				{
					var orderDetail = new List<OrderDetail>
					{
						detail
					};

					await _ordersCollection.InsertOneAsync(new OrderModel()
					{
						ListDetail = orderDetail,
						Phone = phone,
					}); ;
				}
				else
				{
					List<OrderDetail> orders = new List<OrderDetail>(userGetOrder.ListDetail)
					{
						detail
					};
					var filter = Builders<OrderModel>.Filter
	 .Eq(x => x.Phone, phone);
					var update = Builders<OrderModel>.Update
						.Set(y => y.ListDetail, orders);
					await _ordersCollection.UpdateOneAsync
						(filter, update);
				}


				return new JsonResult(new
				{
					Code = 200,
					Data = new { },
					Message = "success"
				});
			}
			catch (Exception e)
			{

				return new JsonResult(new
				{
					Code = 200,
					Data = new { message = e.Message },
					Message = "success"
				});
			}
		}
		private async void PrintImages(List<ImageModel> imgs)
		{

			using (Image<Rgba32> image = new((int)MilimeterToPixel(210), (int)MilimeterToPixel(297)))
			{
				image.Mutate(x => x.BackgroundColor(Color.White));
				foreach (ImageModel im in imgs)
				{

					if (im.Url.Count > 0)
					{
						foreach (var item in im.Url)
						{
                            using (var httpClient = new HttpClient())
                            {
                                //Issue the GET request to a URL and read the response into a 
                                //stream that can be used to load the image
                                var imageContent = await httpClient.GetByteArrayAsync("<your image url>");

                                using (var imageBuffer = new MemoryStream(imageContent))
                                {
                                    using (Image imsc = Image.Load(imageBuffer))
                                    {
                                        if (im.Type == ImageType.t4x6)
                                        {
                                            int w = (int)MilimeterToPixel(40);
                                            int h = (int)MilimeterToPixel(60);
                                            int howManyCountWidth = (int)MilimeterToPixel(210) / w;
                                            int howManyCountHeight = (int)MilimeterToPixel(297) / h;
                                            int fspacingx = (int)MilimeterToPixel(1);
                                            int spacingx = (int)MilimeterToPixel(2);
                                            int fspacingy = fspacingx;
                                            int spacingy = ((int)MilimeterToPixel(297) - (howManyCountHeight * h) - fspacingy) / howManyCountHeight;
                                            imsc.Mutate(c => c.Resize(w, h));
                                            var poin = new Point();
                                            var index = im.Url.IndexOf(item);
                                            var posionx = 0;
                                            var posiony = 0;
                                            var numberMod = index % howManyCountWidth;
                                            var numberNoMod = index / howManyCountWidth;
                                            var totalSpcx = 0;
                                            var totalSpcy = 0;
                                            if (numberMod == 0)
                                            {
                                                totalSpcx = fspacingx;
                                            }
                                            else
                                            {
                                                totalSpcx = fspacingx + numberMod * (w + spacingx);
                                            }
                                            if (numberNoMod == 0)
                                            {
                                                totalSpcy = fspacingy;
                                            }
                                            else
                                            {
                                                totalSpcy = fspacingy + numberNoMod * (h + spacingy);
                                            }
                                            poin.X = totalSpcx;
                                            poin.Y = totalSpcy;
                                            image.Mutate(c => c.DrawImage(imsc, poin, 1));
                                            imsc.Dispose();
                                        }
										else if(im.Type == ImageType.t6x9)
										{

										}
                                        else if (im.Type == ImageType.t9x12)
                                        {

                                        }
                                        else if (im.Type == ImageType.t10x15)
                                        {

                                        }
                                        else if (im.Type == ImageType.t13x18)
                                        {

                                        }
                                        else if (im.Type == ImageType.t15x21)
                                        {

                                        }
                                        else if (im.Type == ImageType.t20x30)
                                        {

                                        }
                                    }

                                    //Do something with image
                                }
                            }

                           
						}
					}
				}
				// Resize the image in place and return it for chaining.
				// 'x' signifies the current image processing context.


				// The library automatically picks an encoder based on the file extension then
				// encodes and write the data to disk.
				// You can optionally set the encoder to choose.

				image.Save("D:\\imgs\\img.png");
				//Process p = new Process();

				//p.StartInfo.Verb = "Print";
				//p.Start();
				image.Dispose();
			}
		}

		private float MilimeterToPixel(int valueMl, float dpi = 221)
		{
			return 0.03937007F * dpi * valueMl * 10;
		}

		

	}
}
