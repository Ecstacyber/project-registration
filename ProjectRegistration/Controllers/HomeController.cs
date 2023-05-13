using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectRegistration.Models;
using System.Diagnostics;

namespace ProjectRegistration.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProjectRegistrationManagementContext _context;
        private readonly ILogger<HomeController> _logger;

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
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Login([Bind("Username,UserPassword")] User user)
        {
            var CheckUser = _context.Users.Where(x => x.Username == user.Username && x.UserPassword == user.UserPassword).FirstOrDefault();
            if (CheckUser != null)
            {
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