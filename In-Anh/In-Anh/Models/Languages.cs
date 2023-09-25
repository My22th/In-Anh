namespace In_Anh.Models
{
    public class Languages
    {
        public string Home { get; set; }
        public string History { get; set; }
        public string Login { get; set; }
        public string ImagePrint { get; set; }
        public string FilePrint { get; set; }
        public string ChooseSize { get; set; }
        public string Print { get; set; }
        public string Image { get; set; }
        public string All { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Province { get; set; }
        public string Ward { get; set; }
        public string Note { get; set; }
        public string Status0 { get; set; }
        public string Status1 { get; set; }
        public string Status2 { get; set; }
        public string Status3 { get; set; }
        public string Status4 { get; set; }
        public string NotHaveOrder { get; set; }
        public string CancleOrder { get; set; }
        public string CancleOrderValid { get; set; }
        public Languages LanguageVN()                                           
        {
            return new Languages()
            {
                Home = "Trang Chủ",
                History ="Lịch Sử Đặt Hàng",
                Login = "Đăng Nhập",
                ImagePrint = "In Ảnh",
                FilePrint = "In File",
                ChooseSize ="Thêm Size",
                Print ="In",
                Image ="Ảnh",
                All ="Tất Cả",
                Address ="Địa Chỉ",
                Name ="Tên",
                Phone = "Số Điện Thoại",
                Province = "Tỉnh Thành",
                Ward ="Quận, Huyện",
                Note = "Ghi Chú",
                Status0 ="Chờ Xác Nhận",
                Status1 ="Đang Xử Lý",
                Status2="Tạm Treo",
                Status3 ="Hoàn Thành",
                NotHaveOrder ="Bạn chưa có đơn hàng",
                CancleOrder ="Hủy Đơn Hàng",
                CancleOrderValid ="Bạn Có Chắc Hủy Đơn Này"
            };
        }

        public Languages LanguageEN()
        {
            return new Languages()
            {
                Home = "Home",
                Login = "Login",History ="History",
                ImagePrint = "Image Print",
                FilePrint = "File Print",
                ChooseSize = "More Size",
                All= "All",
                Image ="Image",
                Print ="Print",
                Address = "Address",
                Name = "Name",
                Phone = "Phone",
                Province = "Province",
                Ward = "Ward",
                Note= "Note",
                Status0="Inactive",
                Status1="Active",
                Status2="Pending",
                Status3="Done",
                NotHaveOrder = "You don't have order",
                CancleOrder ="Cancle Order",
                CancleOrderValid ="You Ready To Cancle Order?"
            };
        }

    }

   
    
}
