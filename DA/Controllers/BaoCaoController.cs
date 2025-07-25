using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using DA.Models;
using DA.Data; // Thay bằng namespace project của bạn

namespace YourNamespace.Controllers
{
    public class BaoCaoController : Controller
    {
        private readonly MyDbContext _context;

        public BaoCaoController(MyDbContext context)
        {
            _context = context;
        }
       
        // 1) Thống kê Hợp đồng
        public IActionResult HopDong()
        {
            ViewBag.Tong = _context.HopDongs.Count();
            ViewBag.HieuLuc = _context.HopDongs.Count(h => h.TrangThai == "Còn hiệu lực");
            ViewBag.HetHan = _context.HopDongs.Count(h => h.TrangThai == "Hết hạn");
            return View();
        }

        // 2) Thống kê Người thuê
        public IActionResult NguoiThue()
        {
            ViewBag.TongNguoiThue = _context.NguoiThues.Count();
            return View();
        }

        // 3) Thống kê Tình trạng phòng
        public IActionResult TinhTrangPhong()
        {
            ViewBag.Trong = _context.Phongs.Count(p => p.TrangThai == "Trống");
            ViewBag.DaThue = _context.Phongs.Count(p => p.TrangThai == "Đang thuê");
            ViewBag.BaoTri = _context.Phongs.Count(p => p.TrangThai == "Bảo trì");
            return View();
        }

        // 4) Thống kê Dịch vụ
        public IActionResult DichVu()
        {
            var dichVu = _context.DichVus
                .Select(d => new
                {
                    d.TenDichVu,
                    SoLuong = _context.ChiTietHoaDons.Where(ct => ct.MaDichVu == d.MaDichVu).Sum(ct => ct.SoLuong)
                }).ToList();

            ViewBag.DichVuTen = dichVu.Select(x => x.TenDichVu).ToArray();
            ViewBag.DichVuSoLuong = dichVu.Select(x => x.SoLuong).ToArray();

            return View();
        }

        // 5) Thống kê Doanh thu
        public IActionResult DoanhThu()
        {
            ViewBag.DoanhThu = _context.HoaDons.Sum(h => h.TongTien);
            return View();
        }
        public IActionResult Index()
        {
            // 1) Thống kê Hợp đồng
            ViewBag.HieuLuc = _context.HopDongs.Count(h => h.TrangThai == "Còn hiệu lực");
            ViewBag.HetHan = _context.HopDongs.Count(h => h.TrangThai == "Hết hạn");
            ViewBag.TongHopDong = ViewBag.HieuLuc + ViewBag.HetHan;

            // 2) Thống kê Người thuê
            ViewBag.TongNguoiThue = _context.NguoiThues.Count();

            // 3) Thống kê Tình trạng phòng
            ViewBag.Trong = _context.Phongs.Count(p => p.TrangThai == "Trống");
            ViewBag.DaThue = _context.Phongs.Count(p => p.TrangThai == "Đang thuê");
            ViewBag.BaoTri = _context.Phongs.Count(p => p.TrangThai == "Bảo trì");

            // 4) Thống kê Dịch vụ tiêu thụ
            var dichVu = _context.DichVus
                .Select(d => new
                {
                    d.TenDichVu,
                    SoLuong = _context.ChiTietHoaDons
                                .Where(ct => ct.MaDichVu == d.MaDichVu)
                                .Sum(ct => (int?)ct.SoLuong) ?? 0
                }).ToList();

            ViewBag.DichVuTen = dichVu.Select(x => x.TenDichVu).ToArray();
            ViewBag.DichVuSoLuong = dichVu.Select(x => x.SoLuong).ToArray();

            // 5) Thống kê Doanh thu
            ViewBag.DoanhThu = _context.HoaDons.Sum(h => (decimal?)h.TongTien) ?? 0;

            return View();
        }
    }
}
