using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DA.Data;
using DA.Models;

namespace DA.Controllers
{
    public class LichSuLuuTrusController : Controller
    {
        private readonly MyDbContext _context;

        public LichSuLuuTrusController(MyDbContext context)
        {
            _context = context;
        }

        // GET: LichSuLuuTrus
        public async Task<IActionResult> Index(int? maNguoiThue, int? maPhong, DateTime? tuNgay, DateTime? denNgay)
        {
            var lichSu = _context.LichSuLuuTrus
                .Include(l => l.NguoiThue)
                .Include(l => l.Phong)
                .AsQueryable();

            if (maNguoiThue.HasValue)
                lichSu = lichSu.Where(l => l.MaNguoiThue == maNguoiThue);

            if (maPhong.HasValue)
                lichSu = lichSu.Where(l => l.MaPhong == maPhong);

            if (tuNgay.HasValue)
                lichSu = lichSu.Where(l => l.NgayBatDau >= tuNgay.Value);

            if (denNgay.HasValue)
                lichSu = lichSu.Where(l => l.NgayKetThuc <= denNgay.Value);

            ViewBag.NguoiThues = new SelectList(_context.NguoiThues, "MaNguoiThue", "HoTen"); // sửa lại property tên hiển thị
            ViewBag.Phongs = new SelectList(_context.Phongs, "MaPhong", "TenPhong");

            return View(await lichSu.ToListAsync());
        }

        // GET: Create
        public IActionResult Create()
        {
            ViewBag.NguoiThues = new SelectList(_context.NguoiThues, "MaNguoiThue", "HoTen");
            ViewBag.Phongs = new SelectList(_context.Phongs, "MaPhong", "TenPhong");
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LichSuLuuTru lichSu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lichSu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.NguoiThues = new SelectList(_context.NguoiThues, "MaNguoiThue", "HoTen", lichSu.MaNguoiThue);
            ViewBag.Phongs = new SelectList(_context.Phongs, "MaPhong", "TenPhong", lichSu.MaPhong);
            return View(lichSu);
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int id)
        {
            var lichSu = await _context.LichSuLuuTrus.FindAsync(id);
            if (lichSu == null)
                return NotFound();

            ViewBag.NguoiThues = new SelectList(_context.NguoiThues, "MaNguoiThue", "HoTen", lichSu.MaNguoiThue);
            ViewBag.Phongs = new SelectList(_context.Phongs, "MaPhong", "TenPhong", lichSu.MaPhong);
            return View(lichSu);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LichSuLuuTru lichSu)
        {
            if (id != lichSu.MaLichSu)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lichSu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LichSuExists(lichSu.MaLichSu))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.NguoiThues = new SelectList(_context.NguoiThues, "MaNguoiThue", "HoTen", lichSu.MaNguoiThue);
            ViewBag.Phongs = new SelectList(_context.Phongs, "MaPhong", "TenPhong", lichSu.MaPhong);
            return View(lichSu);
        }

        // GET: Details
        public async Task<IActionResult> Details(int id)
        {
            var lichSu = await _context.LichSuLuuTrus
                .Include(l => l.NguoiThue)
                .Include(l => l.Phong)
                .FirstOrDefaultAsync(m => m.MaLichSu == id);

            if (lichSu == null)
                return NotFound();

            return View(lichSu);
        }

        // GET: Delete
        public async Task<IActionResult> Delete(int id)
        {
            var lichSu = await _context.LichSuLuuTrus
                .Include(l => l.NguoiThue)
                .Include(l => l.Phong)
                .FirstOrDefaultAsync(m => m.MaLichSu == id);

            if (lichSu == null)
                return NotFound();

            return View(lichSu);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lichSu = await _context.LichSuLuuTrus.FindAsync(id);
            if (lichSu != null)
            {
                _context.LichSuLuuTrus.Remove(lichSu);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool LichSuExists(int id)
        {
            return _context.LichSuLuuTrus.Any(e => e.MaLichSu == id);
        }
    }
}
