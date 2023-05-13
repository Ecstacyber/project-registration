using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectRegistration.Models;

namespace ProjectRegistration.Controllers
{
    public class UsersController : Controller
    {
        private readonly ProjectRegistrationManagementContext _context;

        public UsersController(ProjectRegistrationManagementContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var projectRegistrationManagementContext = _context.Users.Include(u => u.Department);
            return View(await projectRegistrationManagementContext.Where(x => x.Deleted == false).ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
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
        public IActionResult Create()
        {
            ViewData["Department"] = new SelectList(_context.Departments, "Id", "Dname");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,Fullname,DateOfBirth,Username,UserPassword,ImagePath,DepartmentId,UserTypeId")] User user)
        {
            if (ModelState.IsValid)
            {
                user.CreatedDateTime = DateTime.Now;
                user.Username = user.UserId;
                user.UserPassword = user.UserId;
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Department"] = new SelectList(_context.Departments.Where(x => x.Deleted == false), "Dname", "Dname", user.DepartmentId);
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,Fullname,DateOfBirth,Username,UserPassword,ImagePath,DepartmentId,UserTypeId,CreatedDateTime")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Id", user.DepartmentId);
            return View(user);
        }

        //// GET: Users/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null || _context.Users == null)
        //    {
        //        return NotFound();
        //    }

        //    var user = await _context.Users
        //        .Include(u => u.Department)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(user);
        //}

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'ProjectRegistrationManagementContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.Deleted = true;
                user.DeletedDateTime = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpPost, ActionName("AddUserFromFileExcel")]
        [ValidateAntiForgeryToken]
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

                        var user = new User();
                        user.CreatedDateTime = DateTime.Now;
                        user.Fullname = reader.GetValue(2).ToString();
                        user.UserId = reader.GetValue(1).ToString();
                        user.Username = user.UserId;
                        user.UserPassword = RandomPassword(8);
                        user.UserTypeId = 100;
                        var department = _context.Departments.Where(x => x.Dname == reader.GetValue(3).ToString()).FirstOrDefault();
                        if (department != null)
                        {
                            user.DepartmentId = department.Id;
                        }
                        _context.Users.Add(user);
                    }

                }
            }

            System.IO.File.Delete(filepath);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        public string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        public string RandomPassword(int size = 0)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(RandomString(4, true));
            builder.Append(RandomNumber(1000, 9999));
            builder.Append(RandomString(2, false));
            return builder.ToString();
        }
    }
}