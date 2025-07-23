using DA.Data;
using DA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DA.Controllers
{
    public class QLNguoiThueController : Controller
    {
        private readonly MyDbContext _context;

        public QLNguoiThueController(MyDbContext context)
        {
            _context = context;
        }

        // GET: Danh sách người thuê
        public async Task<IActionResult> Index(string searchString)
        {
            var query = _context.NguoiThues.Include(n => n.TaiKhoan).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(nt => nt.HoTen.Contains(searchString));
            }

            ViewData["CurrentFilter"] = searchString;
            return View(await query.ToListAsync());
        }


        // GET: Create - Truyền sẵn MaTaiKhoan từ đăng ký

        // GET: Create
        //public IActionResult Create()
        //{
        //    var danhSachTaiKhoanChuaLienKet = _context.TaiKhoans
        //        .Where(t => t.LoaiTaiKhoan == "NguoiThue" && !_context.NguoiThues.Any(nt => nt.MaTaiKhoan == t.MaTaiKhoan))
        //        .Select(t => new SelectListItem
        //        {
        //            Value = t.MaTaiKhoan.ToString(),
        //            Text = t.MaTaiKhoan.ToString() // 👉 Hiển thị Mã tài khoản
        //        }).ToList();

        //    ViewBag.MaTaiKhoan = danhSachTaiKhoanChuaLienKet;

        //    return View();
        //}

        //public IActionResult Create()
        //{
        //    var danhSachTaiKhoanChuaLienKet = _context.TaiKhoans
        //        .Where(t => t.LoaiTaiKhoan == "NguoiThue"
        //                    && !_context.NguoiThues.Any(nt => nt.MaTaiKhoan == t.MaTaiKhoan))
        //        .Select(t => new SelectListItem
        //        {
        //            Value = t.MaTaiKhoan.ToString(),
        //            Text = t.TenDangNhap // nên hiển thị TenDangNhap, dễ nhìn
        //        }).ToList();

        //    danhSachTaiKhoanChuaLienKet.Insert(0, new SelectListItem
        //    {
        //        Value = "",
        //        Text = "-- Chọn tài khoản --"
        //    });

        //    ViewBag.MaTaiKhoan = danhSachTaiKhoanChuaLienKet;

        //    return View();
        //}


        // POST: Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(NguoiThue nguoiThue)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(nguoiThue);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    // Load lại danh sách nếu có lỗi
        //    var danhSachTaiKhoanChuaLienKet = _context.TaiKhoans
        //        .Where(t => t.LoaiTaiKhoan == "NguoiThue" && !_context.NguoiThues.Any(nt => nt.MaTaiKhoan == t.MaTaiKhoan))
        //        .Select(t => new SelectListItem
        //        {
        //            Value = t.MaTaiKhoan.ToString(),
        //            Text = t.MaTaiKhoan.ToString() // 👉 Hiển thị lại Mã tài khoản khi lỗi
        //        }).ToList();

        //    ViewBag.MaTaiKhoan = danhSachTaiKhoanChuaLienKet;

        //    return View(nguoiThue);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(NguoiThue nguoiThue)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(nguoiThue);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    // Load lại
        //    var danhSachTaiKhoanChuaLienKet = _context.TaiKhoans
        //        .Where(t => t.LoaiTaiKhoan == "NguoiThue"
        //                    && !_context.NguoiThues.Any(nt => nt.MaTaiKhoan == t.MaTaiKhoan))
        //        .Select(t => new SelectListItem
        //        {
        //            Value = t.MaTaiKhoan.ToString(),
        //            Text = t.TenDangNhap
        //        }).ToList();

        //    danhSachTaiKhoanChuaLienKet.Insert(0, new SelectListItem
        //    {
        //        Value = "",
        //        Text = "-- Chọn tài khoản --"
        //    });

        //    ViewBag.MaTaiKhoan = danhSachTaiKhoanChuaLienKet;

        //    return View(nguoiThue);
        //}

        // GET: QLNguoiThue/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var nguoiThue = await _context.NguoiThues.FindAsync(id);
            if (nguoiThue == null) return NotFound();

            return View(nguoiThue);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(NguoiThue nguoiThue)
        {
            // 🔍 BƯỚC 1: Kiểm tra tính hợp lệ của dữ liệu model
            if (!ModelState.IsValid)
            {
                var allErrors = ModelState
                    .Where(ms => ms.Value.Errors.Count > 0)
                    .Select(ms => new { Field = ms.Key, Errors = ms.Value.Errors.Select(e => e.ErrorMessage) })
                    .ToList();

                // In ra log hoặc tạm return về view để kiểm tra
                return Content(System.Text.Json.JsonSerializer.Serialize(allErrors));
            }

            // 🔍 BƯỚC 2: Kiểm tra người thuê có tồn tại trong CSDL không
            var existing = await _context.NguoiThues.FindAsync(nguoiThue.MaNguoiThue);
            if (existing == null)
                return NotFound();

            // 🔁 BƯỚC 3: Cập nhật từng trường (tránh bind nhầm FK như MaTaiKhoan)
            existing.HoTen = nguoiThue.HoTen;
            existing.Email = nguoiThue.Email;
            existing.CCCD = nguoiThue.CCCD;
            existing.SoDienThoai = nguoiThue.SoDienThoai;
            existing.DiaChi = nguoiThue.DiaChi;

            try
            {
                // 💾 Lưu thay đổi vào CSDL
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                // 🧨 Nếu có lỗi khi lưu (thường do ràng buộc FK hoặc dữ liệu trống)
                return Content("Lỗi khi lưu CSDL: " + ex.Message);
            }
        }




        // GET: Delete
        // GET: QLNguoiThue/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var nguoiThue = await _context.NguoiThues
                .Include(nt => nt.TaiKhoan)
                .FirstOrDefaultAsync(nt => nt.MaNguoiThue == id);

            if (nguoiThue == null) return NotFound();

            return View(nguoiThue);
        }

        // POST: QLNguoiThue/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nguoiThue = await _context.NguoiThues.FindAsync(id);
            if (nguoiThue != null)
            {
                _context.NguoiThues.Remove(nguoiThue);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        // GET: Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var nguoiThue = await _context.NguoiThues
                .Include(nt => nt.TaiKhoan)
                .FirstOrDefaultAsync(nt => nt.MaNguoiThue == id);

            if (nguoiThue == null) return NotFound();

            return View(nguoiThue);
        }

        private bool NguoiThueExists(int id)
        {
            return _context.NguoiThues.Any(e => e.MaNguoiThue == id);
        }
    }
}
