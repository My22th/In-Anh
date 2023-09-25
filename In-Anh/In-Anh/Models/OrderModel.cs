using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace In_Anh.Models
{
    public class OrderModel
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public List<OrderDetail> ListDetail { get; set; }

        public int OrderTotal { get; set;}  
        public decimal PricePrice { get; set;}  
        public decimal PriceDiscount { get; set;}   
        public decimal PriceTotalDiscount { get;set;}
        public decimal PriceDiscountTotal { get;set;}
        
        public string Phone { get; set;}
        public DateTime DayUpdate { get; set;}


    }
    public class OrderDetail
    {
        public string OrderId { get; set; }
        public string UserEmail { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
        public DateTime DayOrder { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public Active? Active  { get; set; }
        public List<string> Urls { get; set; }
        public List<ImageModel>? Images { get; set; }
        public string? Port { get; set; }
        public int? Total { get; set; }  
    }
   
    public enum Active
    { 
        Inactive,
        Process,
        Pending,
        Done
    }
}
