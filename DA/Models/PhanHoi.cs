using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DA.Models
{
    [Table("PhanHoi")]
    public class PhanHoi
    {
        [Key]
        public int MaPhanHoi { get; set; }
        [Required]
        public int MaNguoiThue { get; set; }

        [Required]
        public string NoiDung { get; set; }

        public DateTime? NgayGui { get; set; }

        public string? KetQuaXuLy { get; set; }

        public DateTime? NgayXuLy { get; set; }

        // FK

        [ValidateNever]
        [ForeignKey("MaNguoiThue")]
        public NguoiThue NguoiThue { get; set; }
    }

}
