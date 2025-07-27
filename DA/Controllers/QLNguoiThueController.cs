using DA.Data;
using DA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

        // GET: Index
        public async Task<IActionResult> Index(string searchString)
        {
            var query = _context.NguoiThues.Include(n => n.TaiKhoan).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
                query = query.Where(nt => nt.HoTen.Contains(searchString));

            ViewData["CurrentFilter"] = searchString;
            return View(await query.ToListAsync());
        }

        public IActionResult Create()
        {
            // Chọn tài khoản chưa có người thuê gắn kèm
            var taiKhoansChuaLienKet = _context.TaiKhoans
                .Where(tk => !_context.NguoiThues.Any(nt => nt.MaTaiKhoan == tk.MaTaiKhoan))
                .ToList();

            ViewBag.MaTaiKhoan = new SelectList(taiKhoansChuaLienKet, "MaTaiKhoan", "MaTaiKhoan"); // ✅ chọn MaTaiKhoan để hiển thị và chọn

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NguoiThue nguoiThue)
        {
            if (_context.NguoiThues.Any(nt => nt.CCCD == nguoiThue.CCCD))
            {
                ModelState.AddModelError("CCCD", "CCCD đã tồn tại");
            }

            if (!ModelState.IsValid)
            {
                // Khi lỗi thì cần load lại dropdown MaTaiKhoan
                var taiKhoansChuaLienKet = _context.TaiKhoans
                    .Where(tk => !_context.NguoiThues.Any(nt => nt.MaTaiKhoan == tk.MaTaiKhoan))
                    .ToList();

                ViewBag.MaTaiKhoan = new SelectList(taiKhoansChuaLienKet, "MaTaiKhoan", "MaTaiKhoan");
                return View(nguoiThue);
            }

            _context.Add(nguoiThue);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // ✅ EDIT: GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var nguoiThue = await _context.NguoiThues.FindAsync(id);
            if (nguoiThue == null) return NotFound();

            return View(nguoiThue);
        }

        // ✅ EDIT: POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(NguoiThue nguoiThue)
        {
            var existing = await _context.NguoiThues.FindAsync(nguoiThue.MaNguoiThue);
            if (existing == null) return NotFound();

            // Ràng buộc: CCCD trùng nhưng không phải chính mình
            if (_context.NguoiThues.Any(nt => nt.CCCD == nguoiThue.CCCD && nt.MaNguoiThue != nguoiThue.MaNguoiThue))
            {
                ModelState.AddModelError("CCCD", "CCCD đã tồn tại");
            }

            if (!ModelState.IsValid)
            {
                return View(nguoiThue);
            }

            existing.HoTen = nguoiThue.HoTen;
            existing.Email = nguoiThue.Email;
            existing.CCCD = nguoiThue.CCCD;
            existing.SoDienThoai = nguoiThue.SoDienThoai;
            existing.DiaChi = nguoiThue.DiaChi;

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                return Content("Lỗi khi lưu CSDL: " + ex.Message);
            }
        }

        // ✅ DELETE: GET
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var nguoiThue = await _context.NguoiThues
                .Include(nt => nt.TaiKhoan)
                .FirstOrDefaultAsync(nt => nt.MaNguoiThue == id);

            if (nguoiThue == null) return NotFound();

            return View(nguoiThue);
        }

        // ✅ DELETE: POST
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

        // ✅ DETAILS
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
