using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DA.Models
{
    public class Phong_DichVu
    {
        [Key, Column(Order = 0)]
        public int MaPhong { get; set; }
        [Key, Column(Order = 1)]
        public int MaDichVu { get; set; }

        public DateTime NgayApDung { get; set; }
        [ValidateNever]
        [ForeignKey(nameof(MaPhong))]
        public virtual Phong Phong { get; set; }
        [ValidateNever]
        [ForeignKey(nameof(MaDichVu))]
        public virtual DichVu DichVu { get; set; }
    }
}
