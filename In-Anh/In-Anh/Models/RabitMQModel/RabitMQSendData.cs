namespace In_Anh.Models.RabitMQModel
{
    public class RabitMQSendData
    {
        public string OrderID { get; set; }
        public ImageType Type { get; set; }
        public byte[] File { get; set; }
        public string Path { get; set; }
        public string CDNPath { get; set; }
        public string FileName { get; set; }
        public Active TypeActive { get; set; }
    }
}
