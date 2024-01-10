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
using ProjectRegistration.Factory.Interfaces;
using ProjectRegistration.Strategy.Interfaces;
using ProjectRegistration.Strategy;

namespace ProjectRegistration.Controllers
{
    public class UsersController : Controller
    {
        private readonly IDENTITYUSERContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IUserFactory _lecturerUserFactory;
        private readonly IUserFactory _studentUserFactory;
        private IUserListStrategy _userListStrategy;


        public UsersController(IDENTITYUSERContext context,
            UserManager<User> userManager,
            IUserFactory lecturerUserFactory,
            IUserFactory studentUserFactory)
        {
            _context = context;
            _userManager = userManager;
            _lecturerUserFactory = lecturerUserFactory;
            _studentUserFactory = studentUserFactory;
        }


        [Authorize(Roles = "Manager, Lecturer, Student")]
        public async Task<IActionResult> StudentList()
        {
            _userListStrategy = new StudentListStrategy();
            return View(await _userListStrategy.GetUserList(_context));
        }

        [Authorize(Roles = "Manager, Lecturer, Student")]
        public async Task<IActionResult> LecturerList()
        {
            _userListStrategy = new LecturerListStrategy();
            return View(await _userListStrategy.GetUserList(_context));
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
            ViewData["Department"] = new SelectList(_context.Departments.Where(x => x.Deleted == false), "Id", "Info");
            return View();
        }

        [Authorize(Roles = "Manager")]
        public IActionResult CreateLecturer()
        {
            ViewData["Department"] = new SelectList(_context.Departments.Where(x => x.Deleted == false), "Id", "Info");
            return View();
        }

        [Authorize(Roles = "Manager")]
        public IActionResult CreateStudent()
        {
            ViewData["Department"] = new SelectList(_context.Departments.Where(x => x.Deleted == false), "Id", "Info");
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
                IUserFactory userFactory;

                if (user.UserTypeId == 10)
                {
                    userFactory = _lecturerUserFactory;
                }
                else if (user.UserTypeId == 100)
                {
                    userFactory = _studentUserFactory;
                }
                else
                {
                    // Handle other user types or throw an exception
                    return BadRequest("Invalid user type");
                }
                var newUser = await userFactory.CreateUser(user);
                if (newUser != null)
                {
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
            if (user.UserTypeId == 10)
            {
                return RedirectToAction(nameof(LecturerList), user);
            }
            else
            {
                return RedirectToAction(nameof(StudentList), user);
            }
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
            ViewData["Department"] = new SelectList(_context.Departments.Where(x => x.Deleted == false), "Id", "Dname");
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
            ViewData["Department"] = new SelectList(_context.Departments.Where(x => x.Deleted == false), "Id", "Dname");
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
            ViewData["Department"] = new SelectList(_context.Departments.Where(x => x.Deleted == false), "Id", "Dname");
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Manager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,UserId,Gender,Fullname,DateOfBirth,DepartmentId,UserTypeId")] User user, IFormFile ImagePath, string DOB)
        {
            ModelState.Remove("ImagePath");
            if (ModelState.IsValid)
            {
                try
                {
                    var updatedUser = await _userManager.FindByIdAsync(user.Id);

                    // Convert string date "dd / mm / yyyy" to DateTime
                    int day = int.Parse(DOB.Split("/")[0]);
                    int month = int.Parse(DOB.Split("/")[1]);
                    int year = int.Parse(DOB.Split('/')[2]);
                    updatedUser.DateOfBirth = new DateTime(year, month, day);

                    updatedUser.UserId = user.UserId;
                    updatedUser.Fullname = user.Fullname;
                    updatedUser.Gender = user.Gender;


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
            if (user.UserTypeId == 100)
            {
                return RedirectToAction("StudentList");
            }
            else
            {
                return RedirectToAction("LecturerList");
            }
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
                        if (existedUser != null)
                        {
                            existedUser.Fullname = reader.GetValue(2).ToString();
                            continue;
                        }

                        var user = new User();
                        user.UserId = reader.GetValue(1).ToString();
                        user.CreatedDateTime = DateTime.Now;
                        user.Fullname = reader.GetValue(2).ToString();
                        user.UserTypeId = 100;
                        var department = _context.Departments.Where(x => x.Dname == reader.GetValue(3).ToString()).FirstOrDefault();
                        user.DepartmentId = (department != null) ? department.Id : 1;

                        await _studentUserFactory.CreateUser(user);
                    }

                }
            }

            System.IO.File.Delete(filepath);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(StudentList));
        }

    }
}