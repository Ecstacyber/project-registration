using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectRegistration.Models;

namespace ProjectRegistration.Controllers
{
    public class ClassesController : Controller
    {
        private readonly ProjectRegistrationManagementContext _context;

        public ClassesController(ProjectRegistrationManagementContext context)
        {
            _context = context;
        }

        // GET: Classes
        public async Task<IActionResult> Index()
        {
            var projectRegistrationManagementContext = _context.Classes.Include(x => x.Course).Where(x => x.Deleted == false);
            return View(await projectRegistrationManagementContext.ToListAsync());
        }

        // GET: Classes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Classes == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes
                .Include(x => x.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@class == null)
            {
                return NotFound();
            }

            return View(@class);
        }

        // GET: Classes/Create
        public IActionResult Create()
        {
            ViewData["Course"] = new SelectList(_context.Courses, "Id", "CourseName");
            return View();
        }

        // POST: Classes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CourseId,ClassId,Semester,Cyear,CreatedDateTime,Deleted,DeletedDateTime")] Class @class)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@class);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Course"] = new SelectList(_context.Courses, "CourseName", "CourseName", @class.CourseId);
            return View(@class);
        }

        // GET: Classes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Classes == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes.FindAsync(id);
            if (@class == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Id", @class.CourseId);
            return View(@class);
        }

        // POST: Classes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CourseId,ClassId,Semester,Cyear,CreatedDateTime,Deleted,DeletedDateTime")] Class @class)
        {
            if (id != @class.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@class);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassExists(@class.Id))
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
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Id", @class.CourseId);
            return View(@class);
        }

        // GET: Classes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Classes == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes
                .Include(x => x.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@class == null)
            {
                return NotFound();
            }

            return View(@class);
        }

        // POST: Classes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Classes == null)
            {
                return Problem("Entity set 'ProjectRegistrationManagementContext.Classes'  is null.");
            }
            var @class = await _context.Classes.FindAsync(id);
            if (@class != null)
            {
                @class.Deleted = true;
                @class.DeletedDateTime = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClassExists(int id)
        {
            return (_context.Classes?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpPost, ActionName("AddUserFromFileExcel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUserFromFileExcel(int Id, IFormFile fileSelect)
        {
            var fileextension = Path.GetExtension(fileSelect.FileName);
            var filename = Guid.NewGuid().ToString() + fileextension;
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

                        var user = _context.Users.Where(x => (x.UserId == reader.GetValue(1).ToString() && x.Deleted == false)).FirstOrDefault();
                        if (user == null) continue;
                        if (_context.ClassDetails.Where(x => x.ClassId == Id && x.UserId == user.Id).FirstOrDefault() != null) continue;

                        ClassDetail classDetail= new ClassDetail();
                        classDetail.ClassId = Id;
                        classDetail.UserId = user.Id;
                        classDetail.Class = _context.Classes.Where(x => (x.Deleted == false && x.Id == Id)).FirstOrDefault();
                        classDetail.User = user;

                        _context.ClassDetails.Add(classDetail);
                    }
                }
            }

            System.IO.File.Delete(filepath);

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = Id} );
        }
    }
}
