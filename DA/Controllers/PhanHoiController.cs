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
    public class PhanHoiController : Controller
    {
        private readonly MyDbContext _context;

        public PhanHoiController(MyDbContext context)
        {
            _context = context;
        }

        // GET: PhanHoi
        public async Task<IActionResult> Index()
        {
            var list = await _context.PhanHois.ToListAsync();
            return View(list);
        }


        // GET: PhanHoi/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phanHoi = await _context.PhanHois
                .Include(p => p.NguoiThue)
                .FirstOrDefaultAsync(m => m.MaPhanHoi == id);
            if (phanHoi == null)
            {
                return NotFound();
            }

            return View(phanHoi);
        }

        // GET: PhanHoi/Create
        public IActionResult Create()
        {
            ViewData["MaNguoiThue"] = new SelectList(_context.NguoiThues, "MaNguoiThue", "CCCD");
            return View();
        }

        // POST: PhanHoi/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaPhanHoi,MaNguoiThue,NoiDung,NgayGui,KetQuaXuLy,NgayXuLy")] PhanHoi phanHoi)
        {
            if (ModelState.IsValid)
            {
                _context.Add(phanHoi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaNguoiThue"] = new SelectList(_context.NguoiThues, "MaNguoiThue", "CCCD", phanHoi.MaNguoiThue);
            return View(phanHoi);
        }

        // GET: PhanHoi/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phanHoi = await _context.PhanHois.FindAsync(id);
            if (phanHoi == null)
            {
                return NotFound();
            }
            ViewData["MaNguoiThue"] = new SelectList(_context.NguoiThues, "MaNguoiThue", "CCCD", phanHoi.MaNguoiThue);
            return View(phanHoi);
        }

        // POST: PhanHoi/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaPhanHoi,MaNguoiThue,NoiDung,NgayGui,KetQuaXuLy,NgayXuLy")] PhanHoi phanHoi)
        {
            if (id != phanHoi.MaPhanHoi)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(phanHoi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhanHoiExists(phanHoi.MaPhanHoi))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaNguoiThue"] = new SelectList(_context.NguoiThues, "MaNguoiThue", "CCCD", phanHoi.MaNguoiThue);
            return View(phanHoi);
        }

        // GET: PhanHoi/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phanHoi = await _context.PhanHois
                .Include(p => p.NguoiThue)
                .FirstOrDefaultAsync(m => m.MaPhanHoi == id);
            if (phanHoi == null)
            {
                return NotFound();
            }

            return View(phanHoi);
        }

        // POST: PhanHoi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var phanHoi = await _context.PhanHois.FindAsync(id);
            if (phanHoi != null)
            {
                _context.PhanHois.Remove(phanHoi);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PhanHoiExists(int id)
        {
            return _context.PhanHois.Any(e => e.MaPhanHoi == id);
        }
    }
}
