using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DA.Models
{
    [Table("ChiTietHoaDon")]
    public class ChiTietHoaDon
    {
        [Key]
        public int MaChiTiet { get; set; }

        public int MaHoaDon { get; set; }

        public int MaDichVu { get; set; }

        public decimal? SoLuong { get; set; }

        public decimal? DonGia { get; set; }
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal ThanhTien { get; set; }

        // FK
        [ValidateNever]
        [ForeignKey("MaHoaDon")]
        public HoaDon HoaDon { get; set; }
        [ValidateNever]
        [ForeignKey("MaDichVu")]
        public DichVu DichVu { get; set; }
    }

}
