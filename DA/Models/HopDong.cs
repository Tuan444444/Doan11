using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DA.Models
{
    [Table("HopDong")]
    public class HopDong
    {
        [Key]
        public int MaHopDong { get; set; }

        [Required]
        public int MaNguoiThue { get; set; }

        [Required]
        public int MaPhong { get; set; }

        [Required]
        public DateTime? NgayBatDau { get; set; }

        [Required]
        public DateTime? NgayKetThuc { get; set; }

        [Required]
        public decimal TienDatCoc { get; set; }

        [MaxLength(50)]
        public string TrangThai { get; set; }
        [ValidateNever]
        [ForeignKey(nameof(MaNguoiThue))]
        
        public virtual NguoiThue NguoiThue { get; set; }
        [ValidateNever]
        [ForeignKey(nameof(MaPhong))]
        public virtual Phong Phong { get; set; }
        [ValidateNever]
        public virtual ICollection<HoaDon> HoaDons { get; set; }
        [ValidateNever]
        public virtual ICollection<ChiSoDichVu> ChiSoDichVus { get; set; }

    }
}
