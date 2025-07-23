using DA.Data;
using DA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;

namespace DA.Controllers
{
    public class NguoiThueController : Controller
    {
        private readonly MyDbContext _context;

        public NguoiThueController(MyDbContext context)
        {
            _context = context;
        }

        // Dashboard chính
        public IActionResult Dashboard()
        {
            int? maTK = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTK == null) return RedirectToAction("Login", "Account");
           

            var nguoiThue = _context.NguoiThues.FirstOrDefault(x => x.MaTaiKhoan == maTK);
            if (nguoiThue == null) return NotFound();

            // ✅ Lấy hợp đồng trước để sử dụng ở đoạn dưới
            var hopDong = _context.HopDongs
    .Include(h => h.Phong)
        .ThenInclude(p => p.ChuNha) // Thêm dòng này để EF lấy luôn thông tin chủ nhà
    .FirstOrDefault(h => h.MaNguoiThue == nguoiThue.MaNguoiThue && h.TrangThai == "Còn hiệu lực");
          
            Phong phong = null;
            if (hopDong != null)
            {
                // 🔍 Lấy phòng thủ công
                phong = _context.Phongs.FirstOrDefault(p => p.MaPhong == hopDong.MaPhong);
            }
            if (hopDong == null)
                return View(new TrangChuViewModel
                {
                    TenPhong = "Chưa có phòng thuê",
                    TrangThaiHopDong = "Không có hợp đồng",
                    ThongKeDichVu = new List<ThongKeDichVuVM>()
                });

            // ✅ Lấy chủ nhà từ phòng
            var chuNha = _context.ChuNhas
                .Include(c => c.TaiKhoan)
                .FirstOrDefault(c => c.MaChuNha == hopDong.Phong.MaChuNha);

            // ✅ Lấy biểu đồ dịch vụ
            var thongKe = _context.HoaDons
                .Where(h => h.MaHopDong == hopDong.MaHopDong)
                .OrderByDescending(h => h.NgayLap)
                .Take(6)
                .Select(h => new ThongKeDichVuVM
                {
                    ThangNam = h.NgayLap.Month + "/" + h.NgayLap.Year,
                    SoDien = (double)_context.ChiTietHoaDons
                                .Where(ct => ct.MaHoaDon == h.MaHoaDon && ct.MaDichVu == 1)
                                .Sum(ct => ct.SoLuong),
                    SoNuoc = (double)_context.ChiTietHoaDons
                                .Where(ct => ct.MaHoaDon == h.MaHoaDon && ct.MaDichVu == 2)
                                .Sum(ct => ct.SoLuong)
                })
                .ToList();

            // ✅ Gán vào ViewModel
            var vm = new TrangChuViewModel
            {
                TenPhong = hopDong.Phong?.TenPhong,
                NgayBatDau = hopDong.NgayBatDau,
                NgayKetThuc = hopDong.NgayKetThuc,
                TrangThaiHopDong = hopDong.TrangThai,

                TenChuNha = chuNha?.TaiKhoan?.TenDangNhap ?? "Không rõ",
                SDTChuNha = chuNha?.SoDienThoai ?? "",
                EmailChuNha = chuNha?.Email ?? "",

                ThongKeDichVu = thongKe
            };

            return View(vm);
        }



        // GET: Thông tin cá nhân
        [HttpGet]
        public IActionResult ThongTinCaNhan()
        {
            int? maTK = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTK == null)
                return RedirectToAction("Login", "Account");

            var nguoiThue = _context.NguoiThues.FirstOrDefault(x => x.MaTaiKhoan == maTK);
            if (nguoiThue == null)
                return NotFound();

            return View(nguoiThue);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThongTinCaNhan(NguoiThue model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Dữ liệu không hợp lệ!";
                return View(model);
            }

            var nguoiThue = _context.NguoiThues.FirstOrDefault(x => x.MaNguoiThue == model.MaNguoiThue);
            if (nguoiThue == null)
                return NotFound();

            nguoiThue.HoTen = model.HoTen;
            nguoiThue.CCCD = model.CCCD;
            nguoiThue.SoDienThoai = model.SoDienThoai;
            nguoiThue.Email = model.Email;
            nguoiThue.DiaChi = model.DiaChi;

            _context.SaveChanges();
            TempData["Message"] = "Cập nhật thành công!";

            return RedirectToAction("ThongTinCaNhan");
        }

        // GET: Thông tin tài khoản (nếu bạn muốn hiển thị)
        public IActionResult TaiKhoan()
        {
            int? maTK = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTK == null)
                return RedirectToAction("Login", "Account");

            var taiKhoan = _context.TaiKhoans.FirstOrDefault(x => x.MaTaiKhoan == maTK);
            if (taiKhoan == null)
                return NotFound();

            return View(taiKhoan);
        
        }
     

        [HttpPost]
        public IActionResult DoiMatKhau(string MatKhauCu, string MatKhauMoi, string XacNhanMatKhau)
        {
            int? maTK = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTK == null)
                return RedirectToAction("Login", "Account");

            var taiKhoan = _context.TaiKhoans.FirstOrDefault(x => x.MaTaiKhoan == maTK);
            if (taiKhoan == null)
                return NotFound();

            if (taiKhoan.MatKhau != MatKhauCu)
            {
                TempData["Message"] = "❌ Mật khẩu cũ không đúng!";
                return RedirectToAction("ThongTinCaNhan");
            }

            if (MatKhauMoi != XacNhanMatKhau)
            {
                TempData["Message"] = "❌ Mật khẩu xác nhận không khớp!";
                return RedirectToAction("ThongTinCaNhan");
            }

            taiKhoan.MatKhau = MatKhauMoi;
            _context.SaveChanges();

            TempData["Message"] = "✅ Đổi mật khẩu thành công!";
            return RedirectToAction("ThongTinCaNhan");
        }


        // GET: Thông tin lưu trú → sub-menu
        public IActionResult ThongTinPhong()
        {
            int? maTK = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTK == null) return RedirectToAction("Login", "Account");

            var nguoiThue = _context.NguoiThues.FirstOrDefault(x => x.MaTaiKhoan == maTK);
            if (nguoiThue == null) return NotFound();

            var phongData = (from hd in _context.HopDongs
                             join p in _context.Phongs on hd.MaPhong equals p.MaPhong
                             join cn in _context.ChuNhas on p.MaChuNha equals cn.MaChuNha
                             where hd.MaNguoiThue == nguoiThue.MaNguoiThue && hd.TrangThai == "Còn hiệu lực"
                             select new Phong
                             {
                                 TenPhong = p.TenPhong,
                                 LoaiPhong = p.LoaiPhong,
                                 DienTich = p.DienTich,
                                 GiaPhong = p.GiaPhong,
                                 TrangThai = p.TrangThai,
                                 
                             }).FirstOrDefault();

            if (phongData == null)
                ViewBag.Message = "Bạn hiện chưa có phòng đang thuê.";

            return View(phongData);
        }

        public IActionResult ThongTinHopDong()
        {
            int? maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null)
                return RedirectToAction("Login", "Account");

            var nguoiThue = _context.NguoiThues.FirstOrDefault(x => x.MaTaiKhoan == maTaiKhoan);
            if (nguoiThue == null)
                return NotFound();

            var hopDong = _context.HopDongs
                .Include(h => h.Phong) // lấy luôn tên phòng
                .FirstOrDefault(h => h.MaNguoiThue == nguoiThue.MaNguoiThue && h.TrangThai == "Còn hiệu lực");

            if (hopDong == null)
            {
                TempData["Message"] = "Không tìm thấy hợp đồng còn hiệu lực.";
                return View(new HopDong()); // Tránh null
            }

            return View(hopDong);
        }

        // GET: Hóa đơn
        public IActionResult HoaDon()
        {
            int? maTK = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTK == null)
                return RedirectToAction("Login", "Account");

            // Tìm mã người thuê tương ứng
            var nguoiThue = _context.NguoiThues.FirstOrDefault(x => x.MaTaiKhoan == maTK);
            if (nguoiThue == null)
                return NotFound();

            // Lấy danh sách hóa đơn theo hợp đồng của người thuê
            var hoaDons = _context.HoaDons
                .Include(h => h.HopDong)
                    .ThenInclude(hd => hd.Phong) // 🛠️ Thêm dòng này để tránh lỗi null!
                .Where(h => h.HopDong.MaNguoiThue == nguoiThue.MaNguoiThue)
                .OrderByDescending(h => h.NgayLap)
                .ToList();

            return View(hoaDons);
        }

        public IActionResult ChiTietHoaDon(int id)
        {
            var hoaDon = _context.HoaDons
      .Include(h => h.HopDong)
          .ThenInclude(hd => hd.Phong) // <- BẮT BUỘC CÓ!
      .Include(h => h.ChiTietHoaDons)
          .ThenInclude(ct => ct.DichVu)
      .FirstOrDefault(h => h.MaHoaDon == id);

            if (hoaDon == null)
                return NotFound();

            return View(hoaDon);
        }

        public IActionResult XuatHoaDonPdf(int id)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var hoaDon = _context.HoaDons
                .Include(h => h.HopDong).ThenInclude(hd => hd.Phong)
                .Include(h => h.HopDong).ThenInclude(hd => hd.NguoiThue)
                .Include(h => h.ChiTietHoaDons).ThenInclude(ct => ct.DichVu)
                .FirstOrDefault(h => h.MaHoaDon == id);

            if (hoaDon == null)
                return NotFound();

            var tienPhong = hoaDon.HopDong?.Phong?.GiaPhong ?? 0;
            var tongDichVu = hoaDon.ChiTietHoaDons.Sum(ct => ct.ThanhTien);

            // Lấy phí phạt nếu có (giả định có cột hoặc bảng riêng, ở đây đơn giản hóa)
            decimal phiPhat = _context.PhanHois
                .Where(p => p.MaNguoiThue == hoaDon.HopDong.MaNguoiThue && p.KetQuaXuLy.Contains("Phạt"))
                .Sum(p => 50000); // giả định mỗi vi phạm 50k

            var tongTien = tienPhong + tongDichVu + phiPhat;

            var stream = new MemoryStream();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    // Header
                    page.Header().AlignCenter().Column(col =>
                    {
                        col.Item().Text("HỆ THỐNG NHÀ TRỌ ABC").Bold().FontSize(14).FontColor(Colors.Blue.Medium);
                        col.Item().Text("Địa chỉ: 123 Trọ Xanh, TP.HCM").FontSize(10);
                        col.Item().Text("Hotline: 0989 000 999").FontSize(10);
                    });

                    page.Content().PaddingVertical(15).Column(col =>
                    {
                        col.Spacing(10);
                        col.Item().AlignCenter().Text("HÓA ĐƠN THANH TOÁN").FontSize(20).Bold();
                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        // Thông tin hóa đơn
                        col.Item().Text($"Mã hóa đơn: {hoaDon.MaHoaDon}");
                        col.Item().Text($"Ngày lập: {hoaDon.NgayLap:dd/MM/yyyy}");
                        col.Item().Text($"Trạng thái: {hoaDon.TrangThaiThanhToan}");
                        col.Item().Text($"Phòng: {hoaDon.HopDong?.Phong?.TenPhong ?? "N/A"}");
                        col.Item().Text($"Người thuê: {hoaDon.HopDong?.NguoiThue?.HoTen ?? "N/A"}");
                        col.Item().Text($"SĐT: {hoaDon.HopDong?.NguoiThue?.SoDienThoai ?? "N/A"}");

                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        // Dịch vụ - Bảng
                        col.Item().Text("Chi tiết dịch vụ sử dụng:").Bold().FontSize(13);
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2); // Dịch vụ
                                columns.ConstantColumn(40); // SL
                                columns.ConstantColumn(60); // Đơn giá
                                columns.ConstantColumn(80); // Thành tiền
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Dịch vụ").Bold();
                                header.Cell().AlignRight().Text("Số lượng").Bold();
                                header.Cell().AlignRight().Text("Đơn giá").Bold();
                                header.Cell().AlignRight().Text("Thành tiền").Bold();
                            });

                            foreach (var ct in hoaDon.ChiTietHoaDons)
                            {
                                table.Cell().Text(ct.DichVu?.TenDichVu ?? "N/A");
                                table.Cell().AlignRight().Text(ct.SoLuong.ToString());
                                table.Cell().AlignRight().Text($"{ct.DonGia:N0} đ");
                                table.Cell().AlignRight().Text($"{ct.ThanhTien:N0} đ");
                            }
                        });

                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        // Tổng kết cuối cùng
                        col.Item().Text($"Tiền phòng: {tienPhong:N0} đ");
                        col.Item().Text($"Tổng dịch vụ: {tongDichVu:N0} đ");
                        if (phiPhat > 0)
                            col.Item().Text($"Phí phạt: {phiPhat:N0} đ").FontColor(Colors.Red.Medium);
                        col.Item().Text($"Tổng cộng: {tongTien:N0} đ").FontSize(14).Bold().FontColor(Colors.Black);
                    });

                    page.Footer().AlignCenter().Text("Cảm ơn quý khách đã sử dụng dịch vụ!").Italic().FontSize(10).FontColor(Colors.Grey.Darken1);
                });
            });

            document.GeneratePdf(stream);
            stream.Position = 0;

            return File(stream.ToArray(), "application/pdf", $"HoaDon_{hoaDon.MaHoaDon}.pdf");
        }

        // GET: Phản hồi
        public IActionResult PhanHoi()
        {
            return View();
        }
        [HttpPost]
        public IActionResult PhanHoi(PhanHoi phanHoi)
        {
            int? maTK = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTK == null)
                return RedirectToAction("Login", "Account");

            var nguoiThue = _context.NguoiThues.FirstOrDefault(x => x.MaTaiKhoan == maTK);
            if (nguoiThue == null)
                return NotFound();

            phanHoi.MaNguoiThue = nguoiThue.MaNguoiThue;
            phanHoi.NgayGui = DateTime.Now;
            phanHoi.KetQuaXuLy = null;
            phanHoi.NgayXuLy = null;

            _context.PhanHois.Add(phanHoi);
            _context.SaveChanges();

            TempData["Message"] = "Gửi phản hồi thành công.";
            return RedirectToAction("PhanHoi");
        }

    }
}
