using DA.Data;
using DA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DA.Models;

public class TaiKhoanController : Controller
{
    private readonly MyDbContext _context;

    public TaiKhoanController(MyDbContext context)
    {
        _context = context;
    }

    // 1️⃣ Danh sách + Tìm kiếm
    public IActionResult Index(string search)
    {
        var query = _context.TaiKhoans.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(x => x.TenDangNhap.Contains(search));
        }

        var list = query.ToList();
        return View(list);
    }

    // 2️⃣ Sửa - GET
    public IActionResult Edit(int id)
    {
        var tk = _context.TaiKhoans.Find(id);
        if (tk == null) return NotFound();
        return View(tk);
    }

    // 2️⃣ Sửa - POST
    [HttpPost]
    public IActionResult Edit(TaiKhoan model)
    {
        var tk = _context.TaiKhoans.Find(model.MaTaiKhoan);
        if (tk == null) return NotFound();

        tk.TenDangNhap = model.TenDangNhap;
        tk.LoaiTaiKhoan = model.LoaiTaiKhoan;
        tk.TrangThai = model.TrangThai;

        _context.SaveChanges();

        TempData["Message"] = "Cập nhật thành công!";
        return RedirectToAction("Index");
    }

    // 3️⃣ Xoá
    public IActionResult Delete(int id)
    {
        var tk = _context.TaiKhoans.Find(id);
        if (tk == null) return NotFound();

        _context.TaiKhoans.Remove(tk);
        _context.SaveChanges();

        TempData["Message"] = "Đã xoá tài khoản!";
        return RedirectToAction("Index");
    }

    // 4️⃣ Reset mật khẩu
    public IActionResult ResetPassword(int id)
    {
        var tk = _context.TaiKhoans.Find(id);
        if (tk == null) return NotFound();

        tk.MatKhau = "123456"; // Mật khẩu mặc định
        _context.SaveChanges();

        TempData["Message"] = "Đã reset mật khẩu về 123456!";
        return RedirectToAction("Index");
    }
}
