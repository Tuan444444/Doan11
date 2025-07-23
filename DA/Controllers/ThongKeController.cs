using DA.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DA.Controllers
{
    public class ThongKeController : Controller
    {
        private readonly MyDbContext _context;

        public ThongKeController(MyDbContext context)
        {
            _context = context;
        }
        public IActionResult Nguoidung()
        {
            var soLuongChuNha = _context.TaiKhoans.Count(x => x.LoaiTaiKhoan == "ChuNha");
            var soLuongNguoiThue = _context.TaiKhoans.Count(x => x.LoaiTaiKhoan == "NguoiThue");
            var soLuongAdmin = _context.TaiKhoans.Count(x => x.LoaiTaiKhoan == "Admin");

            ViewBag.ChuNha = soLuongChuNha;
            ViewBag.NguoiThue = soLuongNguoiThue;
            ViewBag.Admin = soLuongAdmin;

            return View();

        }
        public IActionResult ThongKePhong()
        {
            // Lấy toàn bộ phòng
            var tongSoPhong = _context.Phongs.Count();

            // Trạng thái
            var soPhongTrong = _context.Phongs.Count(p => p.TrangThai == "Trống");
            var soPhongDangThue = _context.Phongs.Count(p => p.TrangThai == "Đang thuê");
            var soPhongBaoTri = _context.Phongs.Count(p => p.TrangThai == "Bảo trì");

            // Loại phòng
            var thongKeLoaiPhong = _context.Phongs
                .GroupBy(p => p.LoaiPhong)
                .Select(g => new { LoaiPhong = g.Key, SoLuong = g.Count() })
                .ToList();

            // Gửi sang View
            ViewBag.TongSoPhong = tongSoPhong;
            ViewBag.SoPhongTrong = soPhongTrong;
            ViewBag.SoPhongDangThue = soPhongDangThue;
            ViewBag.SoPhongBaoTri = soPhongBaoTri;
            ViewBag.LoaiPhongData = thongKeLoaiPhong;

            return View();
        }
    }
}
