using RabbitMQ.Client;
using System.Net.Sockets;
using System.Net;
using Lucene.Net.Support;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;
using In_Anh.Models.RabitMQModel;
using System.Text.RegularExpressions;
using ImageMagick;
using In_Anh.Models;
using MongoDB.Driver;
using static Google.Cloud.Firestore.V1.StructuredQuery.Types;

namespace In_Anh.RabitMQ
{
    public class RabitMQConsumer : IHostedService
    {

        public IConfiguration _config;

        public IMongoCollection<OrderModel> _ordersCollection;
        public IMongoCollection<ImageModel> _imagesCollection;

        public readonly IRabitMQProducer _rabitMQProducer;


        public RabitMQConsumer(IConfiguration config)
        {
            _config = config;


        }
        private IModel channel = null;
        private IConnection connection = null;
        private void Run()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "jinnie.shop",
                VirtualHost = "/",
                UserName = "admin",
                Password = "admin"
            };
            factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(15);
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: "image",
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            Console.WriteLine(" [*] Waiting for messages.");
            var consumer = new EventingBasicConsumer(this.channel);
            consumer.Received += OnMessageRecieved;
            channel.BasicConsume(queue: "image",
                                autoAck: false,
                                consumer: consumer);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Run();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            channel.Dispose();
            connection.Dispose();
            return Task.CompletedTask;
        }

        private async void OnMessageRecieved(object model, BasicDeliverEventArgs args)
        {
            var body = args.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var data = JsonConvert.DeserializeObject<RabitMQSendData>(message);
            var fileName = data.FileName;
            var filePath = data.Path + "\\" + fileName;
            using (var memoryStream = new MemoryStream(data.File))
            {
                string content = Encoding.UTF8.GetString(data.File);
                if (Regex.IsMatch(content, @"<script|<cross\-domain\-policy",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
                {
                    return;
                }
                if (!Directory.Exists(data.Path))
                {
                    Directory.CreateDirectory(data.Path);
                }
                if (!File.Exists(filePath))
                {
                    using (FileStream f = File.Create(filePath))
                    {
                        f.Dispose();
                    }
                }
                memoryStream.Position = 0;
                using (MagickImage image = new MagickImage(memoryStream))
                {
                    image.Format = MagickFormat.Jpeg;
                    image.Quality = 100;
                    image.Write(filePath);
                    image.Dispose();
                }
                var issv = await SaveImageAsync(data.OrderID, data.CDNPath + fileName, data.Type);

                memoryStream.Close();
            }
            Console.WriteLine(" [x] Done");
            channel.BasicAck(deliveryTag: args.DeliveryTag, multiple: false);
        }

        private async Task<bool> SaveImageAsync(string orderID, string urlImg, ImageType type)
        {
            try
            {
                double price = 0;

                switch (type)
                {
                    case ImageType.t4x6:
                        price = 2000;
                        break;
                    case ImageType.t6x9:
                        price = 3000;
                        break;
                    case ImageType.t9x12:
                        price = 7000;
                        break;
                    case ImageType.t10x15:
                        price = 9000;
                        break;
                    case ImageType.t13x18:
                        price = 12000;
                        break;
                    case ImageType.t15x21:
                        price = 15000;
                        break;
                    case ImageType.t20x30:
                        price = 30000;
                        break;
                    default:
                        break;
                }
                var filter = Builders<OrderModel>.Filter.And(Builders<OrderModel>.Filter.Where(x => x.ListDetail.Any(y => y.OrderId == orderID)));

                var ip = Dns.GetHostEntry(Dns.GetHostName())
       .AddressList
       .First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
       .ToString();
                var port = 0;
                if (ip == "192.168.1.4")
                {
                    port = 444;
                }
                else
                {
                    port = 446;
                }
                var cdn = _config["Cdn:UrlCdn"];

                var client = new MongoClient(port == 444 ? _config["ImageMgDatabase:ConnectionString"] : _config["ImageMgDatabase:ConnectionString1"]);

                var database = client.GetDatabase(_config["ImageMgDatabase:DatabaseName"]);

                _imagesCollection = database.GetCollection<ImageModel>(_config["ImageMgDatabase:ImagesCollectionName"]);
                await _imagesCollection.InsertOneAsync(new ImageModel()
                {
                    Id = orderID,
                    Type = type,
                    Price = price,
                    OrginUrl = new List<string> { cdn + ":" + port + "\\" + urlImg }
                });




                return true;
            }
            catch (Exception e)
            {


            }
            return true;
        }
    }
}
