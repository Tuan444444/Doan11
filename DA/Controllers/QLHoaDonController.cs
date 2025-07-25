using DA.Data;
using DA.Models;
using DA.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DA.Controllers
{
    public class QLHoaDonController : Controller
    {
        private readonly MyDbContext _context;

        public QLHoaDonController(MyDbContext context)
        {
            _context = context;
        }

        // GET: Hóa đơn
        public IActionResult Index()
        {
            var ds = _context.HoaDons
                .Include(h => h.HopDong)
                .ToList();

            return View(ds);
        }

        // GET: Chi tiết hóa đơn
        public IActionResult Details(int id)
        {
            var hoaDon = _context.HoaDons
                .Include(h => h.HopDong)
                    .ThenInclude(hd => hd.Phong)
                .Include(h => h.ChiTietHoaDons)
                    .ThenInclude(ct => ct.DichVu)
                .FirstOrDefault(h => h.MaHoaDon == id);

            if (hoaDon == null) return NotFound();

            ViewBag.GiaPhong = hoaDon.HopDong?.Phong?.GiaPhong ?? 0;

            return View(hoaDon);
        }


        // GET: HoaDon/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var hoaDon = await _context.HoaDons
                .Include(h => h.HopDong)
                .FirstOrDefaultAsync(m => m.MaHoaDon == id);

            if (hoaDon == null)
                return NotFound();

            return View(hoaDon);
        }

        // POST: HoaDon/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed([Bind("MaHoaDon")] HoaDon hoaDon)
        {
            var entity = await _context.HoaDons
                .Include(h => h.ChiTietHoaDons)
                .FirstOrDefaultAsync(h => h.MaHoaDon == hoaDon.MaHoaDon);

            if (entity != null)
            {
                _context.ChiTietHoaDons.RemoveRange(entity.ChiTietHoaDons);
                _context.HoaDons.Remove(entity);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult ChonHopDong()
        {
            var thang = DateTime.Now.Month;
            var nam = DateTime.Now.Year;

            var hopDongsChuaHoaDon = _context.HopDongs
                .Include(h => h.Phong)
                .Include(h => h.NguoiThue)
                .Where(h =>
                    (h.TrangThai == "Đang thuê" || h.TrangThai == "Còn hiệu lực") &&
                    !_context.HoaDons.Any(hd => hd.MaHopDong == h.MaHopDong && hd.NgayLap.Month == thang && hd.NgayLap.Year == nam)
                )
                .Select(h => new SelectListItem
                {
                    Value = h.MaHopDong.ToString(),
                    Text = $"{h.Phong.TenPhong} - {h.NguoiThue.HoTen}"
                })
                .ToList();

            ViewBag.HopDongs = hopDongsChuaHoaDon;

            return View();
        }
        [HttpPost]
        public IActionResult ChonHopDong(int maHopDong)
        {
            if (maHopDong == 0)
            {
                TempData["Error"] = "Vui lòng chọn hợp đồng.";
                return RedirectToAction(nameof(ChonHopDong));
            }

            return RedirectToAction("TaoHoaDon", new { maHopDong });
        }

        // GET: Tạo hóa đơn cho hợp đồng
        [HttpGet]
        public IActionResult TaoHoaDon(int maHopDong)
        {
            var hopDong = _context.HopDongs
                .Include(h => h.NguoiThue)
                .Include(h => h.Phong)
                .Include(h => h.Phong.Phong_DichVus)
                    .ThenInclude(pd => pd.DichVu)
                .FirstOrDefault(h => h.MaHopDong == maHopDong);

            if (hopDong == null) return NotFound();

            var vm = new HoaDonCreateViewModel
            {
                MaHopDong = hopDong.MaHopDong,
                TenNguoiThue = hopDong.NguoiThue?.HoTen ?? "",
                TenPhong = hopDong.Phong?.TenPhong ?? "",
                GiaPhong = (decimal)(hopDong.Phong?.GiaPhong ?? 0),
                DichVus = hopDong.Phong.Phong_DichVus.Select(pd =>
                {
                    var dv = pd.DichVu;
                    var isChiSo = dv.TenDichVu.ToLower().Contains("điện") || dv.TenDichVu.ToLower().Contains("nước");

                    decimal chiSoCu = 0;
                    if (isChiSo)
                    {
                        var cs = _context.ChiSoDichVus
                            .Where(c => c.MaHopDong == hopDong.MaHopDong && c.MaDichVu == dv.MaDichVu)
                            .OrderByDescending(c => c.NgayNhap)
                            .FirstOrDefault();
                        chiSoCu = (decimal)(cs?.ChiSoMoi ?? 0);
                    }

                    return new DichVuInput
                    {
                        MaDichVu = dv.MaDichVu,
                        TenDichVu = dv.TenDichVu,
                        DonGia = (decimal)dv.DonGia,
                        ChiSoCu = (decimal)chiSoCu
                    };
                }).ToList()
            };

            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TaoHoaDon(HoaDonCreateViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            decimal tongTien = (decimal)(vm.GiaPhong + vm.PhiKhac);

            var hoaDon = new HoaDon
            {
                MaHopDong = vm.MaHopDong,
                NgayLap = DateTime.Now,
                TrangThaiThanhToan = "Chưa thanh toán"
            };

            _context.HoaDons.Add(hoaDon);
            _context.SaveChanges();

            foreach (var dv in vm.DichVus)
            {
                bool isChiSo = dv.TenDichVu.ToLower().Contains("điện") || dv.TenDichVu.ToLower().Contains("nước");

                decimal soLuong = 0;
                if (isChiSo)
                {
                    decimal chiSoMoi = Convert.ToDecimal(dv.ChiSoMoi);
                    decimal chiSoCu = Convert.ToDecimal(dv.ChiSoCu);

                    soLuong = Math.Max(0, chiSoMoi - chiSoCu);

                    var chiSo = new ChiSoDichVu
                    {
                        MaHopDong = vm.MaHopDong,
                        MaDichVu = dv.MaDichVu,
                        ChiSoCu = (decimal)chiSoCu,
                        ChiSoMoi = (decimal)chiSoMoi,
                        NgayNhap = DateTime.Now
                    };
                    _context.ChiSoDichVus.Add(chiSo);
                }
                else
                {
                    soLuong = Convert.ToDecimal(dv.SoLuong ?? 0);
                }

                decimal donGia = Convert.ToDecimal(dv.DonGia);
                decimal thanhTien = soLuong * donGia;
                tongTien += thanhTien;

                var ct = new ChiTietHoaDon
                {
                    MaHoaDon = hoaDon.MaHoaDon,
                    MaDichVu = dv.MaDichVu,
                    SoLuong = soLuong,
                    DonGia = (decimal)donGia,
                    ThanhTien = (decimal)thanhTien
                };
                _context.ChiTietHoaDons.Add(ct);
            }

            hoaDon.TongTien = (decimal?)tongTien;
            _context.SaveChanges();

            TempData["Message"] = "Tạo hóa đơn thành công.";
            return RedirectToAction("Index");
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

    }
}

