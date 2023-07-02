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

        public HomeController(ILogger<HomeController> logger, IDENTITYUSERContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult GetProjectCompleteData()
        {
            float totalDoingPj = _context.Projects
                .Include(p => p.Class)
                .Where(x => x.State == "Đang thực hiện" 
                    || x.State == "Đã hoàn thành" 
                    && x.Class.Semester == GetCurrentSemester() 
                    && x.Class.Cyear == GetCurrentYear()
                ).Count();
            if (totalDoingPj == 0) { return Json(0); }
            float uncompletedPj = _context.Projects.Include(p => p.Class).Where(x => x.State == "Đang thực hiện").Count();
            var response = uncompletedPj / totalDoingPj;
            return Json(response);
        }

        public static int GetCurrentSemester()
        {
            int currentMonth = DateTime.Now.Month;
            int currentDay = DateTime.Now.Day;
            if (currentMonth >= 9 && currentMonth == 1)
            {
                if (currentMonth == 9 && currentDay >= 11)
                {
                    return 1;
                }
                if (currentMonth == 1 && currentDay < 29)
                {
                    return 1;
                }
                else return 0;
            }
            if (currentMonth >= 2 && currentMonth <= 7)
            {
                if (currentMonth == 2 && currentDay >= 19)
                {
                    return 2;
                }
                if (currentMonth == 7 && currentDay < 8)
                {
                    return 2;
                }
                else return 0;
            }
            return 0;
        }

        public int GetProjectRegisterdNumber()
        {
            int totalDoingPj = _context.Projects
                .Include(p => p.Class)
                .Where(x => x.State == "Đang thực hiện"
                    || x.State == "Đã hoàn thành"
                    && x.Class.Semester == GetCurrentSemester()
                    && x.Class.Cyear == GetCurrentYear()
                ).Count();
            return totalDoingPj;
        }
        public int GetProjectOnGoingNumber()
        {
            int totalDoingPj = _context.Projects
                .Include(p => p.Class)
                .Where(x => x.State == "Đang thực hiện"
                    && x.Class.Semester == GetCurrentSemester()
                    && x.Class.Cyear == GetCurrentYear()
                ).Count();
            return totalDoingPj;
        }
        public int GetProjectFinishedNumber()
        {
            int totalDoingPj = _context.Projects
                .Include(p => p.Class)
                .Where(x => x.State == "Đã hoàn thành"
                    && x.Class.Semester == GetCurrentSemester()
                    && x.Class.Cyear == GetCurrentYear()
                ).Count();
            return totalDoingPj;
        }

        public static string GetCurrentYear()
        {
            int semester = GetCurrentSemester();
            int currentYear = DateTime.Now.Year;
            if (semester == 2)
            {
                return (currentYear - 1).ToString() + '-' + currentYear.ToString();
            }
            if (semester == 1)
            {
                if (DateTime.Now.Month == 1 && DateTime.Now.Day < 29)
                {
                    return (currentYear - 1).ToString() + '-' + currentYear.ToString();
                }
                else
                {
                    return currentYear.ToString() + '-' + (currentYear + 1).ToString();
                }
            }
            return currentYear.ToString();
        }

        public void CheckData()
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
            if (_context.Courses.Where(x => x.CourseId == "KLTN").FirstOrDefault() == null)
            {
                var course = new Course
                {
                    CourseId = "KLTN",
                    CourseName = "Khóa luận tốt nghiệp",
                    CreatedDateTime = DateTime.Now
                };
                _context.Add(course);
                _context.SaveChanges();

            }
        }


        [Authorize(Roles = "Manager, Lecturer, Student")]
        public IActionResult Index()
        {
            CheckData();
            ViewData["CurrentYear"] = GetCurrentYear();
            ViewData["CurrentSemester"] = GetCurrentSemester();
            ViewData["ProjectRegisteredNumber"] = GetProjectRegisterdNumber();
            ViewData["ProjectOnGoingNumber"] = GetProjectOnGoingNumber();
            ViewData["ProjectFinishedNumber"] = GetProjectFinishedNumber();
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