using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ExcelDataReader;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectRegistration.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace ProjectRegistration.Controllers
{
    public class UsersController : Controller
    {
        private readonly IDENTITYUSERContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _emailStore;
        private readonly IEmailSender _emailSender;

        public UsersController(IDENTITYUSERContext context,
            UserManager<User> userManager,
            IUserStore<User> userStore,
            SignInManager<User> signInManager,
            IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        // GET: Users

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Index()
        {
            var IDENTITYUSERContext = _context.Users.Include(u => u.Department);
            return View(await IDENTITYUSERContext.Where(x => x.Deleted == false).ToListAsync());
        }


        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> StudentList()
        {
            var IDENTITYUSERContext = _context.Users.Where(u => u.UserTypeId == 100).Include(u => u.Department);
            return View(await IDENTITYUSERContext.Where(x => x.Deleted == false).ToListAsync());
        }

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> LecturerList()
        {
            var IDENTITYUSERContext = _context.Users.Where(u => u.UserTypeId == 10).Include(u => u.Department);
            return View(await IDENTITYUSERContext.Where(x => x.Deleted == false).ToListAsync());
        }

        // GET: Users/Details/5

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create

        [Authorize(Roles = "Manager")]
        public IActionResult Create()
        {
            ViewData["Department"] = new SelectList(_context.Departments, "Id", "Info");
            return View();
        }

        [Authorize(Roles = "Manager")]
        public IActionResult CreateLecturer()
        {
            ViewData["Department"] = new SelectList(_context.Departments, "Id", "Info");
            return View();
        }

        [Authorize(Roles = "Manager")]
        public IActionResult CreateStudent()
        {
            ViewData["Department"] = new SelectList(_context.Departments, "Id", "Info");
            return View();
        }
        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Create([Bind("UserId,Fullname,Gender,DateOfBirth,DepartmentId,UserTypeId")] User user, IFormFile ImagePath)
        {
            ModelState.Remove("ImagePath");
            if (_context.Users.Where(x => x.Deleted == false && x.UserId == user.UserId).FirstOrDefault() != null)
            {
                ModelState.AddModelError("UserId", "Mã số sinh viên/giảng viên đã tồn tại");
            }
            if (ModelState.IsValid)
            {
                var newUser = CreateUser();
                newUser.UserId = user.UserId;
                await _userStore.SetUserNameAsync(newUser, newUser.UserId, CancellationToken.None);
                await _emailStore.SetEmailAsync(newUser, newUser.UserId + "@gm.uit.edu.vn", CancellationToken.None);
                var result = await _userManager.CreateAsync(newUser, user.UserId);

                if (result.Succeeded)
                {
                    newUser.CreatedDateTime = DateTime.Now;

                    //var userId = await _userManager.GetUserIdAsync(newUser);
                    //await _signInManager.SignInAsync(newUser, isPersistent: false);

                    newUser.Fullname = user.Fullname;
                    newUser.PhoneNumber = user.PhoneNumber;
                    newUser.Gender = user.Gender;
                    newUser.DateOfBirth = user.DateOfBirth;
                    newUser.DepartmentId = user.DepartmentId;
                    newUser.UserTypeId = user.UserTypeId;

                    if (newUser.UserTypeId == 10)
                    {
                        await _userManager.AddToRoleAsync(newUser, "Lecturer");
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(newUser, "Student");
                    }

                    if (ImagePath != null)
                    {
                        var fileextension = Path.GetExtension(ImagePath.FileName);
                        var filename = Guid.NewGuid().ToString() + fileextension;
                        if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files")))
                        {
                            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files"));
                        }
                        var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", filename);
                        using (FileStream fs = System.IO.File.Create(filepath))
                        {
                            ImagePath.CopyTo(fs);
                        }

                        newUser.ImagePath = filename;
                    }
                    else
                    {
                        newUser.ImagePath = "default-avatar.jpg";
                    }

                    _context.Update(newUser);
                    await _context.SaveChangesAsync();

                    if (newUser.UserTypeId == 10)
                    {
                        return RedirectToAction(nameof(LecturerList));
                    }
                    else
                    {
                        return RedirectToAction(nameof(StudentList));
                    }
                }
            }
            ViewData["Department"] = new SelectList(_context.Departments.Where(x => x.Deleted == false), "Id", "Info", user.DepartmentId);
            return View(nameof(CreateLecturer), user);
        }

        // GET: Users/Edit/5

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["Department"] = new SelectList(_context.Departments, "Id", "Dname");
            return View(user);
        }
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> EditLecturer(string? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["Department"] = new SelectList(_context.Departments, "Id", "Dname");
            return View(user);
        }
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> EditStudent(string? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["Department"] = new SelectList(_context.Departments, "Id", "Dname");
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Manager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,UserId,Gender,Fullname,DateOfBirth,DepartmentId,UserTypeId")] User user, IFormFile ImagePath)
        {
            ModelState.Remove("ImagePath");
            if (_context.Users.Where(x => x.Deleted == false && x.UserId == user.UserId).FirstOrDefault() != null)
            {
                ModelState.AddModelError("UserId", "Mã số sinh viên/giảng viên đã tồn tại");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var updatedUser = await _userManager.FindByIdAsync(user.Id);

                    updatedUser.UserId = user.UserId;
                    updatedUser.Fullname = user.Fullname;
                    updatedUser.Gender = user.Gender;
                    updatedUser.DateOfBirth = user.DateOfBirth;
                    updatedUser.DepartmentId = user.DepartmentId;
                    updatedUser.UserTypeId = user.UserTypeId;

                    if (updatedUser.UserTypeId == 10)
                    {
                        await _userManager.AddToRoleAsync(updatedUser, "Lecturer");
                        await _userManager.RemoveFromRoleAsync(updatedUser, "Student");
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(updatedUser, "Student");
                        await _userManager.RemoveFromRoleAsync(updatedUser, "Lecturer");
                    }

                    if (ImagePath != null)
                    {

                        var fileextension = Path.GetExtension(ImagePath.FileName);
                        var filename = Guid.NewGuid().ToString() + fileextension;
                        if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files")))
                        {
                            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files"));
                        }
                        var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", filename);
                        using (FileStream fs = System.IO.File.Create(filepath))
                        {
                            ImagePath.CopyTo(fs);
                        }

                        updatedUser.ImagePath = filename;
                    }

                    _context.Update(updatedUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if (user.UserTypeId == 10)
                {
                    return RedirectToAction(nameof(LecturerList));
                }
                else
                {
                    return RedirectToAction(nameof(StudentList));
                }
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Id", user.DepartmentId);
            return View(user);
        }

        // POST: Users/Delete/5

        [Authorize(Roles = "Manager")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'IDENTITYUSERContext.Users'  is null.");
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.Deleted = true;
                user.DeletedDateTime = DateTime.Now;
                await _context.SaveChangesAsync();
                if (user.UserTypeId == 10)
                {
                    return RedirectToAction(nameof(LecturerList));
                }
                else
                {
                    return RedirectToAction(nameof(StudentList));
                }
            }
            else
            {
                return RedirectToAction("Home", "ERROR_500", new { id = "" });
            }

        }

        private bool UserExists(string id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpPost, ActionName("AddUserFromFileExcel")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> AddUserFromFileExcel(IFormFile fileSelect)
        {
            var fileextension = Path.GetExtension(fileSelect.FileName);
            var filename = Guid.NewGuid().ToString() + fileextension;
            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files")))
            {
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files"));
            }
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", filename);
            using (FileStream fs = System.IO.File.Create(filepath))
            {
                fileSelect.CopyTo(fs);
            }

            using (var stream = System.IO.File.Open(filepath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    reader.Read();
                    while (reader.Read()) //Each row of the file
                    {

                        var existedUser = _context.Users.Where(x => (x.UserId == reader.GetValue(1).ToString() && x.Deleted == false)).FirstOrDefault();
                        if (existedUser != null) continue;

                        var user = CreateUser();
                        user.UserId = reader.GetValue(1).ToString();
                        await _userStore.SetUserNameAsync(user, user.UserId, CancellationToken.None);
                        await _emailStore.SetEmailAsync(user, user.UserId + "@gm.uit.edu.vn", CancellationToken.None);
                        var result = await _userManager.CreateAsync(user, user.UserId);

                        if (result.Succeeded)
                        {
                            user.CreatedDateTime = DateTime.Now;
                            user.Fullname = reader.GetValue(2).ToString();
                            user.UserTypeId = 100;
                            user.ImagePath = "default-avatar.jpg";
                            await _userManager.AddToRoleAsync(user, "Student");


                            var department = _context.Departments.Where(x => x.Dname == reader.GetValue(3).ToString()).FirstOrDefault();
                            if (department != null)
                            {
                                user.DepartmentId = department.Id;
                            }
                            _context.Update(user);
                        }
                    }

                }
            }

            System.IO.File.Delete(filepath);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private User CreateUser()
        {
            try
            {
                return Activator.CreateInstance<User>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(User)}'. " +
                    $"Ensure that '{nameof(User)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<User> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<User>)_userStore;
        }
    }
}