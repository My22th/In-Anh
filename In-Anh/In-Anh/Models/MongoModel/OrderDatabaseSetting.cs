namespace In_Anh.Models.MongoModel
{
    public class OrderDatabaseSetting

    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string OrderCollectionName { get; set; } = null!;
    }
}
