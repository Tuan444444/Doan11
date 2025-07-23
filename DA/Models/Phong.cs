using DA.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DA.Models
{
    [Table("Phong")]
    public class Phong
    {
        [Key]
        public int MaPhong { get; set; }

        public int MaChuNha { get; set; }

        [Required]
        public string TenPhong { get; set; }

        public string LoaiPhong { get; set; }

        [Required]
        public decimal GiaPhong { get; set; }

        [Required]
        public double DienTich { get; set; }

        public string TrangThai { get; set; }
        [ValidateNever]
        [ForeignKey(nameof(MaChuNha))]
        public virtual ChuNha ChuNha { get; set; }
        [ValidateNever]
        public ICollection<Phong_DichVu> Phong_DichVus { get; set; }
    }
}
