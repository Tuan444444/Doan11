using System.ComponentModel.DataAnnotations;

namespace DA.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class RegisterViewModel
    {

        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }
        public string LoaiTaiKhoan { get; set; } // Admin, ChuNha, NguoiThue
        public string HoTen { get; set; }
        public string CCCD { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string DiaChi { get; set; }
    }


}
