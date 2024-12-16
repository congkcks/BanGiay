using BanGiay.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PagedList;
using System.Diagnostics;
using System.Drawing.Printing;

namespace BanGiay.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly QlbanGiay1Context _context;
		public HomeController(ILogger<HomeController> logger, QlbanGiay1Context context)
		{
			_logger = logger;
			_context = context;
		}

		public IActionResult Index()
		{
			var sanpham = _context.SanPhams.ToList();
			ViewBag.SanPham = sanpham;
			return View();
		}
		public IActionResult Shop()
		{
			var phanloai= _context.PhanLoais.ToList();
			ViewBag.PhanLoai = phanloai;
			return View();
		}
        public IActionResult GetPagedProducts(int? page, int? pageSize)
        {
            int pageNumber = (page ?? 1);
            int size = (pageSize ?? 6);

            var sanpham = _context.SanPhams.OrderBy(p => p.TenSanPham).ToPagedList(pageNumber, size);
            return PartialView("SanPham", sanpham);

        }
		// phan trang va theo ten loai san pham
		public IActionResult GetPagedProductsByCategory(int? page, int? pageSize, string? category)
		{
            int pageNumber = (page ?? 1);
            int size = (pageSize ?? 6);
			if (category == null)
			{
				var sp = _context.SanPhams.OrderBy(p => p.TenSanPham).ToPagedList(pageNumber, size);
                return PartialView("SanPham", sp);
			}
			var maPhanLoai = _context.PhanLoais.Where(p => p.PhanLoaiChinh == category).FirstOrDefault().MaPhanLoai;
            var sanpham = _context.SanPhams.Where(p => p.MaPhanLoai == maPhanLoai).OrderBy(p => p.TenSanPham).ToPagedList(pageNumber, size);
            return PartialView("SanPham", sanpham);
        }
        public IActionResult GetMaPhanLoaiPhuByMaPhanLoai(string maPhanLoai)
        {
            var maPhanLoaiPhu = _context.PhanLoaiPhus
                                         .Where(plp => plp.MaPhanLoai == maPhanLoai)
                                         .ToList();

            // Trả về một danh sách chọn
            return Json(new SelectList(maPhanLoaiPhu, "MaPhanLoaiPhu", "TenPhanLoaiPhu"));
        }
		public IActionResult Search(string? search)
		{
			int pageNumber = 1;
			int size = 10;
            var sanpham = _context.SanPhams.Where(p => p.TenSanPham.Contains(search)).ToPagedList(pageNumber, size);
			return PartialView("SanPham", sanpham);
		}
		[HttpGet]
        public IActionResult ThemSanPham()
		{
			var phanloai = _context.PhanLoais.ToList();
			ViewBag.MaPhanLoai = new SelectList(phanloai, "MaPhanLoai", "PhanLoaiChinh");
			return View();
		}
		[HttpPost]
		public IActionResult ThemSanPham(SanPham sanpham)
		{
			_context.SanPhams.Add(sanpham);
			_context.SaveChanges();
			return RedirectToAction("Index");
		}

        public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
