using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
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
                .Where(x => x.Deleted == false)
                .Include(x => x.Course)
                .Include(x => x.ClassDetails)
                .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@class == null)
            {
                return NotFound();
            }
            var list = @class.ClassDetails.Where(x => x.Deleted == false).ToList();
            @class.ClassDetails = list;
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
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null || _context.Classes == null)
        //    {
        //        return NotFound();
        //    }

        //    var @class = await _context.Classes
        //        .Include(x => x.Course)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (@class == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(@class);
        //}

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
                    var @class = _context.Classes.Where(x => (x.Deleted == false && x.Id == Id)).FirstOrDefault();

                    List<ClassDetail> cd = new List<ClassDetail>();

                    while (reader.Read()) //Each row of the file
                    {

                        var user = _context.Users.Where(x => (x.UserId == reader.GetValue(1).ToString() && x.Deleted == false)).FirstOrDefault();
                        if (user == null) continue;
                        if (_context.ClassDetails.Where(x => x.ClassId == Id && x.UserId == user.Id).FirstOrDefault() != null) continue;

                        ClassDetail classDetail = new ClassDetail();
                        classDetail.ClassId = Id;
                        classDetail.UserId = user.Id;
                        classDetail.Class = @class;
                        classDetail.User = user;
                        classDetail.CreatedDateTime = DateTime.Now;
                        List<ClassDetail> userCd = new List<ClassDetail>();
                        userCd = user.ClassDetails.ToList();
                        userCd.Add(classDetail);
                        user.ClassDetails = userCd;
                        cd.Add(classDetail);                       
                        _context.ClassDetails.Add(classDetail);
                    }
                    @class.ClassDetails = cd;

                }
            }

            System.IO.File.Delete(filepath);

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = Id} );
        }

        [HttpPost, ActionName("GetUsersInClass")]
        public IActionResult GetUsersInClass(int id)
        {
            return RedirectToAction("GetUsersInClass", "ClassDetailsController", id);
        }

        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (_context.Classes == null)
            {
                return Problem("Entity set 'ProjectRegistrationManagementContext.Classes'  is null.");
            }            
            string[] ids = id.Split('-');
            var @classDetails = _context.ClassDetails.Where(x => x.UserId == int.Parse(ids[0]) && x.ClassId == int.Parse(ids[1])).FirstOrDefault();
            if (@classDetails != null)
            {
                @classDetails.Deleted = true;
                @classDetails.DeletedDateTime = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = int.Parse(ids[1]) });
        }

        [HttpPost, ActionName("AddUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser(string id)
        {
            if (_context.Classes == null)
            {
                return Problem("Entity set 'ProjectRegistrationManagementContext.Classes'  is null.");
            }
            string[] ids = id.Split('-');

            var user = _context.Users.Where(x => (x.UserId == ids[0] && x.Deleted == false)).FirstOrDefault();            
            var @class = _context.Classes.Where(x => (x.Id == int.Parse(ids[1]) && x.Deleted == false)).Include(x => x.ClassDetails).FirstOrDefault();
            if (!@class.ClassDetails.Any(x => x.UserId == user.Id))
            {
                ClassDetail classDetail = new ClassDetail();
                classDetail.ClassId = @class.Id;
                classDetail.UserId = user.Id;
                classDetail.Class = @class;
                classDetail.User = user;
                classDetail.CreatedDateTime = DateTime.Now;
                _context.ClassDetails.Add(classDetail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = int.Parse(ids[1]) });
        }

        [HttpPost, ActionName("ViewProjectList")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ViewProjectList(int id)
        {
            var @class = _context.Classes.Where(x => x.Deleted == false && x.Id == id).Include(x => x.ProjectClasses).FirstOrDefault();
            var projectList = new List<Project>();
            projectList = @class.ProjectClasses.ToList();
            //ViewData["id"] = id;
            return View(projectList);
        }

        // GET: Projects/Create
        public IActionResult CreateProject(int id)
        {
            ViewData["id"] = id;
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Id");
            ViewData["ClassId2"] = new SelectList(_context.Classes, "Id", "Id");
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Id");
            ViewData["GradingLecturerId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["GuidingLecturerId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("CreateProject")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProject([Bind("Id,Pname,Info,DepartmentId,ClassId,ClassId2,GuidingLecturerId,GradingLecturerId,Pyear,Semester,CreatedDateTime,Deleted,DeletedDateTime")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.Id = 0;
                project.ClassId = (int?)ViewData["id"];
                project.CreatedDateTime = DateTime.Now;
                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction("ViewProjectList", new { id = ViewData["id"]});
            }
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Id", project.ClassId);
            ViewData["ClassId2"] = new SelectList(_context.Classes, "Id", "Id", project.ClassId2);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Id", project.DepartmentId);
            ViewData["GradingLecturerId"] = new SelectList(_context.Users, "Id", "Id", project.GradingLecturerId);
            ViewData["GuidingLecturerId"] = new SelectList(_context.Users, "Id", "Id", project.GuidingLecturerId);
            return RedirectToAction("ViewProjectList", new { id = ViewData["id"] });
        }
    }
}
