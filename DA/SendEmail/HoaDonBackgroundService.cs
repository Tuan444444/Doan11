using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DA.Data;
using DA.Models;
using DA.SendEmail;

public class HoaDonBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly MailHelper _mailHelper;

    public HoaDonBackgroundService(IServiceProvider serviceProvider, MailHelper mailHelper)
    {
        _serviceProvider = serviceProvider;
        _mailHelper = mailHelper;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var today = DateTime.Now;

            if (today.Day == 3)
            {
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<MyDbContext>();

                var hopDongs = db.HopDongs
                    .Include(h => h.NguoiThue)
                    .Include(h => h.Phong)
                    .Include(h => h.Phong.Phong_DichVus)
                        .ThenInclude(pd => pd.DichVu)
                    .Where(h => h.TrangThai == "Đang thuê" || h.TrangThai == "Còn hiệu lực")
                    .ToList();

                foreach (var hd in hopDongs)
                {
                    bool daCo = db.HoaDons.Any(h =>
                        h.MaHopDong == hd.MaHopDong &&
                        h.NgayLap.Month == today.Month &&
                        h.NgayLap.Year == today.Year);

                    if (!daCo)
                    {
                        decimal tongTien = hd.Phong.GiaPhong;

                        var hoaDon = new HoaDon
                        {
                            MaHopDong = hd.MaHopDong,
                            NgayLap = today,
                            TrangThaiThanhToan = "Chưa thanh toán",
                        };
                        db.HoaDons.Add(hoaDon);
                        db.SaveChanges();

                        foreach (var pd in hd.Phong.Phong_DichVus)
                        {
                            decimal soLuong = 1; // Hoặc tính tùy bạn
                            decimal donGia = Convert.ToDecimal(pd.DichVu.DonGia);
                            decimal thanhTien = soLuong * donGia;

                            var chiTiet = new ChiTietHoaDon
                            {
                                MaHoaDon = hoaDon.MaHoaDon,
                                MaDichVu = pd.DichVu.MaDichVu,
                                SoLuong = soLuong,
                                DonGia = donGia,
                                ThanhTien = thanhTien
                            };
                            db.ChiTietHoaDons.Add(chiTiet);

                            tongTien += thanhTien;
                        }

                        hoaDon.TongTien = tongTien;
                        db.SaveChanges();

                        // Gửi email
                        string email = hd.NguoiThue.Email;
                        string hoTen = hd.NguoiThue.HoTen;

                        if (!string.IsNullOrEmpty(email))
                        {
                            string subject = $"Hóa đơn tháng {hoaDon.NgayLap:MM/yyyy}";
                            string body = $@"
                                Xin chào {hoTen},<br/>
                                Đây là hóa đơn tháng {hoaDon.NgayLap:MM/yyyy}.<br/>
                                Tổng tiền: {hoaDon.TongTien:N0} VND.<br/>
                                Vui lòng thanh toán trước hạn.<br/>
                                Trân trọng.";
                            await _mailHelper.SendMailAsync(email, subject, body);
                        }
                    }
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
