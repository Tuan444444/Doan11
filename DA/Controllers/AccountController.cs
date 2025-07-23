using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using DA.Models;
using DA.ViewModels;
using DA.Data;
using System.Linq;
using System.Threading.Tasks;

public class AccountController : Controller
{
    private readonly MyDbContext _context;

    public AccountController(MyDbContext context)
    {
        _context = context;
    }

    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        // Kiểm tra trùng tên đăng nhập
        bool exist = await _context.TaiKhoans.AnyAsync(x => x.TenDangNhap == model.TenDangNhap);
        if (exist)
        {
            ModelState.AddModelError("", "Tên đăng nhập đã tồn tại");
            return View(model);
        }

        // Tạo tài khoản
        var taiKhoan = new TaiKhoan
        {
            TenDangNhap = model.TenDangNhap,
            MatKhau = model.MatKhau,
            LoaiTaiKhoan = model.LoaiTaiKhoan,
            TrangThai = "Hoạt động",
            NgayTao = DateTime.Now
        };

        _context.TaiKhoans.Add(taiKhoan);
        await _context.SaveChangesAsync(); // Lưu trước để có MaTaiKhoan

        // Tạo người dùng tương ứng
        if (model.LoaiTaiKhoan == "ChuNha")
        {
            var cn = new ChuNha
            {
                MaTaiKhoan = taiKhoan.MaTaiKhoan,
                HoTen = model.HoTen,
                CCCD = model.CCCD,
                SoDienThoai = model.SoDienThoai,
                Email = model.Email,
                DiaChi = model.DiaChi
            };
            _context.ChuNhas.Add(cn);
        }
        else if (model.LoaiTaiKhoan == "NguoiThue")
        {
            var nt = new NguoiThue
            {
                MaTaiKhoan = taiKhoan.MaTaiKhoan,
                HoTen = model.HoTen,
                CCCD = model.CCCD,
                SoDienThoai = model.SoDienThoai,
                Email = model.Email,
                DiaChi = model.DiaChi
            };
            _context.NguoiThues.Add(nt);
        }

        await _context.SaveChangesAsync(); // Lưu người dùng

        TempData["Message"] = "Đăng ký thành công!";
        return RedirectToAction("Login");
    }

    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var tk = await _context.TaiKhoans
            .FirstOrDefaultAsync(x => x.TenDangNhap == model.TenDangNhap && x.MatKhau == model.MatKhau);

        // 1️⃣ Phải check tk null trước
        if (tk == null || tk.TrangThai == "Bị khóa")
        {
            ModelState.AddModelError("", "Đăng nhập thất bại hoặc tài khoản bị khóa");
            return View(model);
        }

        // 2️⃣ Tồn tại tk thì mới lấy chuNha
        var chuNha = await _context.ChuNhas
            .FirstOrDefaultAsync(cn => cn.MaTaiKhoan == tk.MaTaiKhoan);

        HttpContext.Session.SetInt32("MaTaiKhoan", tk.MaTaiKhoan);
        HttpContext.Session.SetString("VaiTro", tk.LoaiTaiKhoan);

        // 3️⃣ Nếu VaiTro là Chủ Nhà thì lưu MaChuNha (nếu có)
        if (tk.LoaiTaiKhoan == "ChuNha" && chuNha != null)
        {
            HttpContext.Session.SetInt32("MaChuNha", chuNha.MaChuNha);
        }

        TempData["Message"] = $"Đăng nhập thành công: {tk.LoaiTaiKhoan}";

        return tk.LoaiTaiKhoan switch
        {
            "Admin" => RedirectToAction("Index", "Admin"),
            "ChuNha" => RedirectToAction("Index", "Chu"),
            _ => RedirectToAction("Dashboard", "NguoiThue")
        };
    }


    public IActionResult DangXuat()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Account");
    }
}
