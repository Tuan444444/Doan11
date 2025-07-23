using Microsoft.AspNetCore.Mvc;
using DA.Data;
using DA.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DA.Controllers
{
    public class PhongController : Controller
    {
        private readonly MyDbContext _context;

        public PhongController(MyDbContext context)
        {
            _context = context;
        }

        // Danh sách phòng
        public IActionResult Index(string search, string trangThai)
        {
            // Lấy MaChuNha từ Session
            int? MaChuNha = HttpContext.Session.GetInt32("MaChuNha");

            if (MaChuNha == null)
            {
                // Chưa đăng nhập thì quay về login
                return RedirectToAction("Login", "Account");
            }

            var dsPhong = _context.Phongs.Where(p => p.MaChuNha == MaChuNha);

            if (!string.IsNullOrEmpty(search))
            {
                dsPhong = dsPhong.Where(p => p.TenPhong.Contains(search));
            }

            if (!string.IsNullOrEmpty(trangThai) && trangThai != "Tất cả")
            {
                dsPhong = dsPhong.Where(p => p.TrangThai == trangThai);
            }

            ViewBag.Search = search;
            ViewBag.TrangThai = trangThai;

            return View(dsPhong.ToList());
        }




        // Tạo mới
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.DichVus = _context.DichVus.ToList(); // danh sách dịch vụ
            return View();
        }

        // POST: Phong/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Phong phong, List<int> SelectedDichVus)
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            Console.WriteLine("MaTaiKhoan trong session: " + maTaiKhoan);


            if (maTaiKhoan == null)
                return RedirectToAction("Login", "Account");

            
     

            var chuNha = _context.ChuNhas.FirstOrDefault(c => c.MaTaiKhoan == maTaiKhoan);

            if (chuNha == null)
            {
                ModelState.AddModelError("", "Không tìm thấy thông tin chủ nhà.");
                ViewBag.DichVus = _context.DichVus.ToList();
                return View(phong);
            }

            phong.MaChuNha = chuNha.MaChuNha;

            if (ModelState.IsValid)
            {
                _context.Phongs.Add(phong);
                _context.SaveChanges();

                // Lưu danh sách dịch vụ đi kèm
                if (SelectedDichVus != null && SelectedDichVus.Any())
                {
                    foreach (var maDv in SelectedDichVus)
                    {
                        _context.Phong_DichVus.Add(new Phong_DichVu
                        {
                            MaPhong = phong.MaPhong,
                            MaDichVu = maDv,
                            NgayApDung = DateTime.Now
                        });
                    }

                    _context.SaveChanges();
                }

                return RedirectToAction("Index");
            }

            // Nếu có lỗi
            ViewBag.DichVus = _context.DichVus.ToList();
            return View(phong);
        }


        // Sửa
        public IActionResult Edit(int id)
        {
            var phong = _context.Phongs.Find(id);
            return View(phong);
        }

        [HttpPost]
        public IActionResult Edit(Phong phong)
        {
            if (ModelState.IsValid)
            {
                var phongCu = _context.Phongs.FirstOrDefault(p => p.MaPhong == phong.MaPhong);
                if (phongCu != null)
                {
                    phongCu.TenPhong = phong.TenPhong;
                    phongCu.LoaiPhong = phong.LoaiPhong;
                    phongCu.GiaPhong = phong.GiaPhong;
                    phongCu.DienTich = phong.DienTich;
                    phongCu.TrangThai = phong.TrangThai;

                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
                return NotFound();
            }
            return View(phong);
        }


        // Xóa
        public IActionResult Delete(int id)
        {
            var phong = _context.Phongs.Find(id);
            if (phong != null)
            {
                _context.Phongs.Remove(phong);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // Quản lý lịch sử lưu trú
        public IActionResult LichSu(int maPhong)
        {
            var ls = _context.LichSuLuuTrus
                             .Where(x => x.MaPhong == maPhong)
                             .Include(x => x.NguoiThue)
                             .ToList();
            return View(ls);
        }

    }
}
