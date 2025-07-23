using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DA.Models
{
    [Table("ChiSoDichVu")]
    public class ChiSoDichVu
    {
        [Key]
        public int MaChiSo { get; set; }

        [Required]
        public int MaHopDong { get; set; }

        [Required]
        public int MaDichVu { get; set; }

        [Required]
        public decimal ChiSoCu { get; set; }

        [Required]
        public decimal ChiSoMoi { get; set; }

        public DateTime NgayNhap { get; set; } = DateTime.Now;

        // Navigation properties
        [ValidateNever]
        [ForeignKey("MaHopDong")]
        public HopDong HopDong { get; set; }

        [ValidateNever]
        [ForeignKey("MaDichVu")]
        public DichVu DichVu { get; set; }
    }
}
