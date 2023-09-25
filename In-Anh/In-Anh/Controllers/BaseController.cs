using In_Anh.Models;
using In_Anh.Models.MongoModel;
using In_Anh.RabitMQ;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Xml.Linq;

namespace In_Anh.Controllers
{

    public  class BaseController : Controller
    {
        public IConfiguration _config;
        public readonly IMongoCollection<UserModel> _usersCollection;
        public readonly IMongoCollection<OrderModel> _ordersCollection;
        public readonly IMongoCollection<HistoryModel> _historysCollection;
        public readonly IMongoCollection<ImageModel> _imagesCollection;
        public readonly IRabitMQProducer _rabitMQProducer;

        public BaseController(IConfiguration config, IImageMgDatabase setting, IRabitMQProducer rabitMQProducer)
        {
            _config = config;
            var client = new MongoClient(_config["ImageMgDatabase:ConnectionString"]);
            var database = client.GetDatabase(_config["ImageMgDatabase:DatabaseName"]);
            _usersCollection = database.GetCollection<UserModel>(_config["ImageMgDatabase:UserCollectionName"]);
            _ordersCollection = database.GetCollection<OrderModel>(_config["ImageMgDatabase:OrderCollectionName"]);
            _historysCollection = database.GetCollection<HistoryModel>(_config["ImageMgDatabase:HistoryCollectionName"]);
            _imagesCollection = database.GetCollection<ImageModel>(_config["ImageMgDatabase:ImagesCollectionName"]);
            _rabitMQProducer = rabitMQProducer;

        }


        public string GetUserCookies()
        {
            var userRaw = Request.Cookies["userToken"];
            if (userRaw == null || userRaw == "")
            {
                return "";
            }
            else
            {
                return userRaw;


            }

        }
        public string GetOrderIdUserCookies()
        {
            var id = Request.Cookies["userOrderTemp"];
            if (id == null || id == "")
            {
                return "";
            }
            else
            {
                return id;
            }

        }
        public string[] GetOrderIdsUserCookies()
        {
            var id = Request.Cookies["userOrder"];
            if (id == null || id == "")
            {
                return null;
            }
            else
            {
                return id.Split(';').ToArray();
            }

        }
        public string GetPhoneUserCookies()
        {
            var id = Request.Cookies["userPhone"];
            if (id == null || id == "")
            {
                return "";
            }
            else
            {
                return id;
            }

        }

        public bool isLogin()
        {
            if (string.IsNullOrEmpty(GetUserCookies()))
            {
                return false;
            }
            return true;
        }

       
        public UserModel GetUserValid(string token)
        {
                        
            var user = new UserModel();
            var mySecret = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
            var mySecurityKey = new SymmetricSecurityKey(mySecret);
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _config["Jwt:Issuer"],
                    ValidAudience = _config["Jwt:Issuer"],
                    IssuerSigningKey = mySecurityKey,
                }, out SecurityToken validatedToken);

                var tokenS = tokenHandler?.ReadToken(token) as JwtSecurityToken;
                user.Email = tokenS?.Claims?.First(claim => claim.Type == "email")?.Value;
                user.UserName = tokenS?.Claims?.First(claim => claim.Type == "name")?.Value;
                user.PhoneNumber = tokenS?.Claims?.First(claim => claim.Type == "phone")?.Value;
                user.ImageUrlUser = tokenS?.Claims?.First(claim => claim.Type == "website")?.Value;
            }
            catch
            {
                return new UserModel();
            }
            return user;
        }

        public void SetCookies(string key, string value, int? expireTime)
        {
            CookieOptions option = new CookieOptions();

            if (expireTime.HasValue)
                option.Expires = DateTime.Now.AddMinutes(expireTime.Value);
            else
                option.Expires = DateTime.Now.AddMilliseconds(10);

            Response.Cookies.Append(key, value, option);
        }
        public void RemoveCookies(string key)
        {
            Response.Cookies.Delete(key);
        }
        //protected ISomeType SomeMember { get; set; }
    }
    


}

