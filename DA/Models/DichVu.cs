using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DA.Models
{
    [Table("DichVu")]
    public class DichVu
    {
        [Key]
        public int MaDichVu { get; set; }

        [Required]
        [MaxLength(100)]
        public string TenDichVu { get; set; }

        public decimal DonGia { get; set; }

        [MaxLength(50)]
        public string DonViTinh { get; set; }

        // Quan hệ ngược với ChiTietHoaDon
        [ValidateNever]
        public ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; }

        // Quan hệ ngược với Phong_DichVu
        [ValidateNever]
        public ICollection<Phong_DichVu> Phong_DichVus { get; set; }
        [ValidateNever]
        public virtual ICollection<ChiSoDichVu> ChiSoDichVus { get; set; }

    }

}
