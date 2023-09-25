using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace In_Anh.Models
{
    public class HistoryModel
    {
        [BsonId]
        public ObjectId _Id { get; set; }
        public int Id { get; set; }
        public string UserEmail { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string OrderID { get; set; }

    }
}
