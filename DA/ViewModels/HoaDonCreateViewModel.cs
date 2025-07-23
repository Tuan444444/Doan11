using DA.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;

namespace DA.ViewModels
{
    public class HoaDonCreateViewModel
    {
        public int MaHopDong { get; set; }
        [ValidateNever]
        public string TenNguoiThue { get; set; }
        [ValidateNever]
        public string TenPhong { get; set; }
        public decimal GiaPhong { get; set; }
        [ValidateNever]
        public List<DichVuInput> DichVus { get; set; }

        public decimal PhiKhac { get; set; } = 0;
    }

    public class DichVuInput
    {
        public int MaDichVu { get; set; }
        public string TenDichVu { get; set; }
        public decimal DonGia { get; set; }
        public decimal ChiSoMoi { get; set; }
        public decimal ChiSoCu { get; set; }
        public int? SoLuong { get; set; }
    }
}
