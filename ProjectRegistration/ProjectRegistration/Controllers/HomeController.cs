using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectRegistration.Models;
using System.Diagnostics;

namespace ProjectRegistration.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDENTITYUSERContext _context;

        public HomeController(ILogger<HomeController> logger,
            IDENTITYUSERContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Authorize(Roles = "Manager, Lecturer, Student")]
        public IActionResult Index()
        {
            if (_context.Departments.Count() == 0)
            {
                var Dnamelist = new string[] { "CNTT", "KHMT", "CNPM", "MMT&TT", "KH&KTTT", "HTTT" };
                var DescList = new string[] { "Công nghệ thông tin", "Khoa học máy tính", "Công nghệ phần mềm", "Mạng máy tính & Truyền thông", "Khoa học & Kỹ thuật thông tin", "Hệ thống thông tin" };
                for (int i = 0; i < Dnamelist.Length; i++)
                {
                    var department = new Department
                    {
                        Dname = Dnamelist[i],
                        Info = DescList[i],
                        CreatedDateTime = DateTime.Now,
                        Deleted = false
                    };
                    _context.Add(department);
                }
                _context.SaveChanges();
            }

            if (_context.Courses.Where(x => x.CourseId == "SE121").FirstOrDefault() == null)
            {
                var course = new Course
                {
                    CourseId = "SE121",
                    CourseName = "Đồ án 1",
                    CreatedDateTime = DateTime.Now
                };
                _context.Add(course);
                _context.SaveChanges();

            }
            if (_context.Courses.Where(x => x.CourseId == "SE122").FirstOrDefault() == null)
            {
                var course = new Course
                {
                    CourseId = "SE122",
                    CourseName = "Đồ án 2",
                    CreatedDateTime = DateTime.Now
                };
                _context.Add(course);
                _context.SaveChanges();

            }
            if (_context.Courses.Where(x => x.CourseId == "SE123").FirstOrDefault() == null)
            {
                var course = new Course
                {
                    CourseId = "SE123",
                    CourseName = "Đồ án 3",
                    CreatedDateTime = DateTime.Now
                };
                _context.Add(course);
                _context.SaveChanges();

            }
            return View();

        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult ERROR_500()
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