using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace In_Anh.Models
{
    public class ImageModel
    {
        [BsonId]
        public ObjectId _Id { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Url { get; set; }
        public List<string>? OrginUrl { get; set; }
        public ImageType Type { get; set; }
        public double? Price { get; set; }
        public string? Port { get; set; }

    }
    public enum ImageType
    {
        [Display(Name = "4x6")]
        t4x6 = 0,
        [Display(Name = "6x9")]
        t6x9 =1,
        [Display(Name = "9x12")]
        t9x12 = 2,
        [Display(Name = "10x15")]
        t10x15 = 3,
        [Display(Name = "13x18")]
        t13x18 = 4,
        [Display(Name = "15x21")]
        t15x21 = 5,
        [Display(Name = "20x30 (~A4)")]
        t20x30 = 6

    }
    
}
