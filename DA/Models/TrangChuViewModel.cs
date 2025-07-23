namespace DA.Models
{
    public class TrangChuViewModel
    {
        // Thông tin phòng và hợp đồng
        public string TenPhong { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public string TrangThaiHopDong { get; set; }

        // Thông tin chủ nhà
        public string TenChuNha { get; set; }
        public string SDTChuNha { get; set; }
        public string EmailChuNha { get; set; }

        // Dữ liệu biểu đồ dịch vụ
        public List<ThongKeDichVuVM> ThongKeDichVu { get; set; }
    }

    public class ThongKeDichVuVM
    {
        public string ThangNam { get; set; }
        public double SoDien { get; set; }
        public double SoNuoc { get; set; }
    }
}
