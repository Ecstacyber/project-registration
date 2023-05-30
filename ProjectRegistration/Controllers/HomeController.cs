using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectRegistration.Models;
using System.Diagnostics;
using System.Net;

namespace ProjectRegistration.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProjectRegistrationManagementContext _context;
        private readonly ILogger<HomeController> _logger;
        //public static User? user;

        public HomeController(ILogger<HomeController> logger, ProjectRegistrationManagementContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            
            var admin = _context.Users.Where(x => x.Username == "admin").FirstOrDefault();
            if (admin == null)
            {
                admin = new User();
                admin.Username = "admin";
                admin.UserPassword = "admin";
                admin.UserTypeId = 1;
                _context.Users.Add(admin);
                _context.SaveChanges();
            }

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
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Login([Bind("Username,UserPassword")] User user)
        {
            var CheckUser = _context.Users.Where(x => x.Username == user.Username && x.UserPassword == user.UserPassword).FirstOrDefault();
            if (CheckUser != null)
            {
                //user = CheckUser;
                IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
                Debug.WriteLine("\n" + "\n" + "\n" + "\n" + "\n" + "\n" + "\n" + "\n" + "\n" + localIPs[1].ToString());
                ViewData["userID"] = CheckUser.Id;
                return RedirectToAction("Index", "Home");
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