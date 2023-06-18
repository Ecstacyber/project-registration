using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly RoleManager<IdentityRole> _roleManager;

        public HomeController(ILogger<HomeController> logger, UserManager<User> userManager, IDENTITYUSERContext context, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _context = context;
            _roleManager = roleManager;
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
                    var department = new Department();
                    department.Dname = Dnamelist[i];
                    department.Info = DescList[i];
                    department.CreatedDateTime = DateTime.Now;
                    department.Deleted = false;
                    _context.Add(department);
                }
                _context.SaveChanges();
            }

            var role = _roleManager.Roles.ToList();

            if (role.Find(x => x.Name == "Manager") == null)
            {
                var mngRole = new IdentityRole();
                mngRole.Id = "Manager";
                mngRole.Name = "Manager";
                mngRole.NormalizedName = "MANAGER";
                _roleManager.CreateAsync(mngRole);
            }

            if (role.Find(x => x.Name == "Lecturer") == null)
            {
                var lectRole = new IdentityRole();
                lectRole.Id = "Lecturer";
                lectRole.Name = "Lecturer";
                lectRole.NormalizedName = "LECTURER";
                _roleManager.CreateAsync(lectRole);
            }

            if (role.Find(x => x.Name == "Student") == null)
            {
                var stuRole = new IdentityRole();
                stuRole.Id = "Student";
                stuRole.Name = "Student";
                stuRole.NormalizedName = "STUDENT";
                _roleManager.CreateAsync(stuRole);
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