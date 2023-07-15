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

        [Authorize(Roles = "Manager, Lecturer, Student")]
        public IActionResult Index()
        {
            CheckData();
            ViewData["CurrentYear"] = GetCurrentYear();
            ViewData["CurrentSemester"] = GetCurrentSemester();
            ViewData["ProjectRegisteredNumber"] = GetProjectRegisterdNumber();
            ViewData["ProjectOnGoingNumber"] = GetProjectOnGoingNumber();
            ViewData["ProjectFinishedNumber"] = GetProjectFinishedNumber();
            ViewData["SE121Grade"] = GetSE121Grade();
            return View();

        }

        public static int GetCurrentSemester()
        {
            int currentMonth = DateTime.Now.Month;
            int currentDay = DateTime.Now.Day;
            if (currentMonth >= 9 && currentMonth <= 2)
            {
                if (currentMonth == 9 && currentDay >= 11)
                {
                    return 1;
                }
                if (currentMonth == 2 && currentDay < 19)
                {
                    return 1;
                }
                else return 2;
            }
            else return 2;
        }

        public int GetProjectRegisterdNumber()
        {
            int totalDoingPj = _context.Projects
                .Include(p => p.Class)
                .Where(x => (x.State == "Đang thực hiện"
                    || x.State == "Đã hoàn thành")
                    && x.Class.Semester == GetCurrentSemester()
                    && x.Class.Cyear == GetCurrentYear()
                ).ToList().Count();
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
                return (currentYear - 1).ToString() + " - " + currentYear.ToString();
            }
            if (semester == 1)
            {
                if (DateTime.Now.Month <= 2 && DateTime.Now.Day < 19)
                {
                    return (currentYear - 1).ToString() + " - " + currentYear.ToString();
                }
                else
                {
                    return currentYear.ToString() + " - " + (currentYear + 1).ToString();
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

        public class GradeDetail
        {
            public int A { get; set; } = 0;
            public int B { get; set; } = 0;
            public int C { get; set; } = 0;
            public int D { get; set; } = 0;
            public int E { get; set; } = 0;
            public int Total { get; set; } = 0;
        }
        public GradeDetail GetSE121Grade()
        {
            var data = new GradeDetail();
            var finishedPj = _context.Projects.Include(x => x.Class)
                .Where(x => x.Deleted == false
                && x.State == "Đã hoàn thành"
                && x.Class.ClassId == "SE121"
                && x.Class.Semester == GetCurrentSemester()
                && x.Class.Cyear == GetCurrentYear());
            
            foreach (var x in finishedPj)
            {
                data.Total++;
                switch (x.PGrade) {
                    case var expression when x.PGrade >= 9:
                        data.A++;
                        break;
                    case var expression when (x.PGrade >= 7 && x.PGrade < 9):
                        data.B++;
                        break;
                    case var expression when (x.PGrade >= 5 && x.PGrade < 7):
                        data.C++;
                        break;
                    case var expression when (x.PGrade >= 0 && x.PGrade < 5):
                        data.D++;
                        break;
                    default:
                        data.E++;
                        break;
                }
            }
            return data;
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