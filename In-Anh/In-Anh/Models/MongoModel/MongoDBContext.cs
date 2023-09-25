public class ImageMgDatabase :IImageMgDatabase
{
    public string OrderCollectionName { get; set; }
    public string UserCollectionName { get; set; }
    public string HistoryCollectionName { get; set; }
    public string ImagesCollectionName { get; set; }
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
}
public interface IImageMgDatabase
{
     string OrderCollectionName { get; set; }
     string UserCollectionName { get; set; }
     string HistoryCollectionName { get; set; }
    public string ImagesCollectionName { get; set; }
    string ConnectionString { get; set; }
     string DatabaseName { get; set; }
}