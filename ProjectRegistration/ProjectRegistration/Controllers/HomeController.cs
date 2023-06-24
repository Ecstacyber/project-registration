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
                var courseIds = new string[] { "SE121", "SE122", "SE123" };
                var courseNames = new string[] { "Đồ án 1", "Đồ án 2", "Đồ án 3" };
                for (int i = 0; i < courseIds.Length; i++)
                {
                    var course = new Course
                    {
                        CourseId = courseIds[i],
                        CourseName = courseNames[i],
                        CreatedDateTime = DateTime.Now,
                        Deleted = false
                    };
                    _context.Add(course);
                }
                _context.SaveChanges();
            }
            if (_context.Departments.Count() > 0 && _context.Courses.Count() == 0)
            {
                var courseIds = new string[] { "SE121", "SE122", "SE123" };
                var courseNames = new string[] { "Đồ án 1", "Đồ án 2", "Đồ án 3" };
                for (int i = 0; i < courseIds.Length; i++)
                {
                    var course = new Course
                    {
                        CourseId = courseIds[i],
                        CourseName = courseNames[i],
                        CreatedDateTime = DateTime.Now,
                        Deleted = false
                    };
                    _context.Add(course);
                }
                _context.SaveChanges();
            }
            return View();

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