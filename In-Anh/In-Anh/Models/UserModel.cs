using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace In_Anh.Models
{
    public class UserModel
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }   
        public string PhoneNumber { get; set; }
       public List<AddressUser> AddressUsers { get; set; }
        public string Notes { get; set; }
        public string ImageUrlUser { get; set; }
        public string Uid { get; set; }

    }
    public class AddressUser
    {
        public string Address { get; set; }
        public string Ward { get; set; }
        public string District { get; set; }
        public string Province { get; set; }

    }
}
