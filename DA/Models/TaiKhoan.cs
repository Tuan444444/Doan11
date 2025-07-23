using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DA.Models
{
    public class TaiKhoan
    {
        [Key]
        public int MaTaiKhoan { get; set; }

        [Required]
        public string TenDangNhap { get; set; }

        [Required]
        public string MatKhau { get; set; }

        [Required]
        public string LoaiTaiKhoan { get; set; } // Admin | ChuNha | NguoiThue

        [Required]
        public string TrangThai { get; set; } // Hoạt động / Bị khóa

        public DateTime NgayTao { get; set; }

        // Điều hướng
        public virtual ChuNha ChuNha { get; set; }
        public virtual NguoiThue NguoiThue { get; set; }
    }
}
