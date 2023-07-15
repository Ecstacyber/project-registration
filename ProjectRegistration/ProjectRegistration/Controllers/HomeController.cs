using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectRegistration.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Schema;

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
            var currentSemester = GetCurrentSemester();
            ViewData["CurrentYear"] = GetCurrentYear();
            ViewData["CurrentSemester"] = currentSemester;
            ViewData["ProjectRegisteredNumber"] = GetProjectRegisterdNumber();
            ViewData["ProjectOnGoingNumber"] = GetProjectOnGoingNumber();
            ViewData["ProjectFinishedNumber"] = GetProjectFinishedNumber();

            var se121 = GetSE121Grade();
            ViewData["SE121_A"] = se121.A;
            ViewData["SE121_B"] = se121.B;
            ViewData["SE121_C"] = se121.C;
            ViewData["SE121_D"] = se121.D;
            ViewData["SE121_E"] = se121.E;
            ViewData["SE121_Total"] = se121.Total;

            var se122 = GetSE122Grade();
            ViewData["SE122_A"] = se122.A;
            ViewData["SE122_B"] = se122.B;
            ViewData["SE122_C"] = se122.C;
            ViewData["SE122_D"] = se122.D;
            ViewData["SE122_E"] = se122.E;
            ViewData["SE122_Total"] = se122.Total;

            var kltn = GetKLTNGrade();
            ViewData["KLTN_A"] = kltn.A;
            ViewData["KLTN_B"] = kltn.B;
            ViewData["KLTN_C"] = kltn.C;
            ViewData["KLTN_D"] = kltn.D;
            ViewData["KLTN_E"] = kltn.E;
            ViewData["KLTN_Total"] = kltn.Total;

            var se121_avgGrade = GetAverageSE121Grade();
            ViewData["SE121_AVG_0"] = se121_avgGrade[0];
            ViewData["SE121_AVG_1"] = se121_avgGrade[1];
            ViewData["SE121_AVG_2"] = se121_avgGrade[2];
            ViewData["SE121_AVG_3"] = se121_avgGrade[3];
            ViewData["SE121_AVG_4"] = se121_avgGrade[4];

            var se122_avgGrade = GetAverageSE122Grade();
            ViewData["SE122_AVG_0"] = se122_avgGrade[0];
            ViewData["SE122_AVG_1"] = se122_avgGrade[1];
            ViewData["SE122_AVG_2"] = se122_avgGrade[2];
            ViewData["SE122_AVG_3"] = se122_avgGrade[3];
            ViewData["SE122_AVG_4"] = se122_avgGrade[4];

            var kltn_avgGrade = GetAverageKLTNGrade();
            ViewData["KLTN_AVG_0"] = kltn_avgGrade[0];
            ViewData["KLTN_AVG_1"] = kltn_avgGrade[1];
            ViewData["KLTN_AVG_2"] = kltn_avgGrade[2];
            ViewData["KLTN_AVG_3"] = kltn_avgGrade[3];
            ViewData["KLTN_AVG_4"] = kltn_avgGrade[4];

            if (currentSemester == 2)
            {
                ViewData["Semester_and_Year_0"] = "HK2 " + GetPastYear(0);
                ViewData["Semester_and_Year_1"] = "HK1 " + GetPastYear(0);
                ViewData["Semester_and_Year_2"] = "HK2 " + GetPastYear(1);
                ViewData["Semester_and_Year_3"] = "HK1 " + GetPastYear(1);
                ViewData["Semester_and_Year_4"] = "HK2 " + GetPastYear(2);
            }
            else
            {
                ViewData["Semester_and_Year_0"] = "HK1 " + GetPastYear(0);
                ViewData["Semester_and_Year_1"] = "HK2 " + GetPastYear(1);
                ViewData["Semester_and_Year_2"] = "HK1 " + GetPastYear(1);
                ViewData["Semester_and_Year_3"] = "HK2 " + GetPastYear(2);
                ViewData["Semester_and_Year_4"] = "HK1 " + GetPastYear(2);
            }
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
                .Where(x => x.Deleted == false && x.Class.Deleted == false &&
                    (x.State == "Đang thực hiện"
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
                .Where(x => x.Deleted == false && x.Class.Deleted == false
                    && x.State == "Đang thực hiện"
                    && x.Class.Semester == GetCurrentSemester()
                    && x.Class.Cyear == GetCurrentYear()
                ).Count();
            return totalDoingPj;
        }
        public int GetProjectFinishedNumber()
        {
            int totalDoingPj = _context.Projects
                .Include(p => p.Class)
                .Where(x => x.Deleted == false && x.Class.Deleted == false
                    && x.State == "Đã hoàn thành"
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

        public static string GetPastYear(int i)
        {
            int currentYear = DateTime.Now.Year - i;
            return (currentYear - 1).ToString() + " - " + currentYear.ToString();
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
            public GradeDetail()
            {

            }
            public GradeDetail(float zero)
            {
                Total = -1;
            }
            public float A { get; set; } = 0;
            public float B { get; set; } = 0;
            public float C { get; set; } = 0;
            public float D { get; set; } = 0;
            public float E { get; set; } = 0;
            public int Total { get; set; } = 0;
        }
        public GradeDetail GetSE121Grade()
        {
            var data = new GradeDetail();
            var finishedPj = _context.Projects.Include(x => x.Class).ThenInclude(x => x.Course)
                .Where(x => x.Deleted == false && x.Class.Deleted == false && x.Class.Course.Deleted == false
                && x.State == "Đã hoàn thành"
                && x.Class.Course.CourseId == "SE121"
                && x.Class.Semester == GetCurrentSemester()
                && x.Class.Cyear == GetCurrentYear());

            foreach (var x in finishedPj)
            {
                data.Total++;
                switch (x.PGrade)
                {
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
            if (data.Total != 0)
            {
                data.A = data.A / data.Total;
                data.B = data.B / data.Total;
                data.C = data.C / data.Total;
                data.D = data.D / data.Total;
                data.E = data.E / data.Total;
                return data;
            }
            else return new GradeDetail(0);
        }
        public GradeDetail GetSE122Grade()
        {
            var data = new GradeDetail();
            var finishedPj = _context.Projects.Include(x => x.Class).ThenInclude(x => x.Course)
                .Where(x => x.Deleted == false && x.Class.Deleted == false && x.Class.Course.Deleted == false
                && x.State == "Đã hoàn thành"
                && x.Class.Course.CourseId == "SE122"
                && x.Class.Semester == GetCurrentSemester()
                && x.Class.Cyear == GetCurrentYear());

            foreach (var x in finishedPj)
            {
                data.Total++;
                switch (x.PGrade)
                {
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
            if (data.Total != 0)
            {
                data.A = data.A / data.Total;
                data.B = data.B / data.Total;
                data.C = data.C / data.Total;
                data.D = data.D / data.Total;
                data.E = data.E / data.Total;
                return data;
            }
            else return new GradeDetail(0);
        }
        public GradeDetail GetKLTNGrade()
        {
            var data = new GradeDetail();
            var finishedPj = _context.Projects.Include(x => x.Class).ThenInclude(x => x.Course)
                .Where(x => x.Deleted == false && x.Class.Deleted == false && x.Class.Course.Deleted == false
                && x.State == "Đã hoàn thành"
                && x.Class.Course.CourseId == "KLTN"
                && x.Class.Semester == GetCurrentSemester()
                && x.Class.Cyear == GetCurrentYear());

            foreach (var x in finishedPj)
            {
                data.Total++;
                switch (x.PGrade)
                {
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
            if (data.Total != 0)
            {
                data.A = data.A / data.Total;
                data.B = data.B / data.Total;
                data.C = data.C / data.Total;
                data.D = data.D / data.Total;
                data.E = data.E / data.Total;
                return data;
            }
            else return new GradeDetail(0);
        }

        public double GetGrade(string courseId, int semester, string year)
        {
            double avgGrade = 0;
            int total = 0;
            var listGrade = _context.Projects.Include(x => x.Class).ThenInclude(x => x.Course)
                .Where(x => x.Deleted == false && x.Class.Deleted == false && x.Class.Course.Deleted == false
                && x.Class.Course.CourseId == courseId
                && x.Class.Semester == semester
                && x.Class.Cyear == year
                && x.State == "Đã hoàn thành"
                ).ToList();

            foreach (var item in listGrade)
            {
                total++;
                avgGrade += (double)item.PGrade;
            }
            if (total != 0) avgGrade /= total;
            return avgGrade;
        }

        public List<double> GetAverageSE121Grade()
        {
            var list = new List<double>();
            var currentSemester = GetCurrentSemester();
            if (currentSemester == 2)
            {
                list.Add(GetGrade("SE121", 2, GetPastYear(2)));
                list.Add(GetGrade("SE121", 1, GetPastYear(1)));
                list.Add(GetGrade("SE121", 2, GetPastYear(1)));
                list.Add(GetGrade("SE121", 1, GetCurrentYear()));
                list.Add(GetGrade("SE121", 2, GetCurrentYear()));
            }
            
            else
            {
                list.Add(GetGrade("SE121", 1, GetPastYear(2)));
                list.Add(GetGrade("SE121", 2, GetPastYear(2)));
                list.Add(GetGrade("SE121", 1, GetPastYear(1)));
                list.Add(GetGrade("SE121", 2, GetPastYear(1)));
                list.Add(GetGrade("SE121", 1, GetCurrentYear()));
            }
            return list;
        }
        public List<double> GetAverageSE122Grade()
        {
            var list = new List<double>();
            var currentSemester = GetCurrentSemester();
            if (currentSemester == 2)
            {
                list.Add(GetGrade("SE122", 2, GetPastYear(2)));
                list.Add(GetGrade("SE122", 1, GetPastYear(1)));
                list.Add(GetGrade("SE122", 2, GetPastYear(1)));
                list.Add(GetGrade("SE122", 1, GetCurrentYear()));
                list.Add(GetGrade("SE122", 2, GetCurrentYear()));
            }

            else
            {
                list.Add(GetGrade("SE122", 1, GetPastYear(2)));
                list.Add(GetGrade("SE122", 2, GetPastYear(2)));
                list.Add(GetGrade("SE122", 1, GetPastYear(1)));
                list.Add(GetGrade("SE122", 2, GetPastYear(1)));
                list.Add(GetGrade("SE122", 1, GetCurrentYear()));
            }
            return list;
        }
        public List<double> GetAverageKLTNGrade()
        {
            var list = new List<double>();
            var currentSemester = GetCurrentSemester();
            if (currentSemester == 2)
            {
                list.Add(GetGrade("KLTN", 2, GetPastYear(2)));
                list.Add(GetGrade("KLTN", 1, GetPastYear(1)));
                list.Add(GetGrade("KLTN", 2, GetPastYear(1)));
                list.Add(GetGrade("KLTN", 1, GetCurrentYear()));
                list.Add(GetGrade("KLTN", 2, GetCurrentYear()));
            }
            
            else
            {
                list.Add(GetGrade("KLTN", 1, GetPastYear(2)));
                list.Add(GetGrade("KLTN", 2, GetPastYear(2)));
                list.Add(GetGrade("KLTN", 1, GetPastYear(1)));
                list.Add(GetGrade("KLTN", 2, GetPastYear(1)));
                list.Add(GetGrade("KLTN", 1, GetCurrentYear()));
            }
            return list;
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