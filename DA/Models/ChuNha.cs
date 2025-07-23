using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DA.Models
{
    [Table("ChuNha")]
    public class ChuNha
    {
        [Key]
        public int MaChuNha { get; set; }

        public int MaTaiKhoan { get; set; } // FK

        public string HoTen { get; set; }

        public string CCCD { get; set; }

        public string SoDienThoai { get; set; }

        public string Email { get; set; }

        public string DiaChi { get; set; }

        [ForeignKey(nameof(MaTaiKhoan))]
        public virtual TaiKhoan TaiKhoan { get; set; } // Điều hướng

        public virtual ICollection<Phong> Phongs { get; set; }
    }
}
