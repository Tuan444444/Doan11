using DA.Data;
using DA.SendEmail;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;


public class HoaDonBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public HoaDonBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine($"[LOG] HoaDonBackgroundService chạy lúc: {DateTime.Now}");

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<MyDbContext>();
                    var mail = scope.ServiceProvider.GetRequiredService<DA.SendEmail.MailHelper>();

                    var nguoiThues = await db.NguoiThues.ToListAsync();

                    foreach (var nt in nguoiThues)
                    {
                        if (string.IsNullOrEmpty(nt.Email)) continue;

                        
                       
                        string subject = $"🧾 Hóa đơn tháng ";
                        string body = $"Xin chào {nt.HoTen},\nĐây là hóa đơn tháng này.\nHãy kiểm tra chi tiết trong hệ thống.";

                        await mail.SendMail(nt.Email, subject, body);

                        Console.WriteLine($"[LOG] Đã gửi cho: {nt.Email}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
