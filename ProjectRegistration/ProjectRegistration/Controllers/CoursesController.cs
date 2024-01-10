using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectRegistration.Command.ConcreteCommands;
using ProjectRegistration.Command.CoursesCommand;
using ProjectRegistration.Command.Invoker;
using ProjectRegistration.Models;

namespace ProjectRegistration.Controllers
{
    public class CoursesController : Controller
    {
        private readonly IDENTITYUSERContext _context;
        private static CoursesCommandInvoker _coursesCommandInvoker = new CoursesCommandInvoker();

        public CoursesController(IDENTITYUSERContext context)
        {
            _context = context;
        }

        // GET: Courses
        [Authorize(Roles = "Manager, Lecturer, Student")]
        public async Task<IActionResult> Index()
        {
              return _context.Courses != null ? 
                          View(await _context.Courses.Where(x => x.Deleted == false).ToListAsync()) :
                          Problem("Entity set 'IDENTITYUSERContext.Courses' is null.");
        }

        // GET: Courses/Details/5
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Details(int id)
        {
            _coursesCommandInvoker.SetCoursesCommand(new CourseDetailsCommand(_context, id));
            Course course = await _coursesCommandInvoker.ExecuteCoursesCommand();
            if (course != null)
            {
                return View(course);
            }
            else
            {
                return NotFound();
            }
        }

        // GET: Courses/Create
        [Authorize(Roles = "Manager")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Create([Bind("Id,CourseId,CourseName,Semester,Cyear,CreatedDateTime,Deleted,DeletedDateTime")] Course course)
        {

            if (ModelState.IsValid)
            {
                _coursesCommandInvoker.SetCoursesCommand(new AddCourseCommand(_context, course));
                Course result = await _coursesCommandInvoker.ExecuteCoursesCommand();
                if (result != null)
                {
                    TempData["message"] = "CourseCreated";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("CourseId", "Mã môn học đã tồn tại");
                    return NotFound();
                }
            }                      
            return View(course);
        }

        // GET: Courses/Edit/5
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Courses == null)
            {
                TempData["message"] = "NoCourseToEdit";
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                TempData["message"] = "NoCourseToEdit";
                return NotFound();
            }
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CourseId,CourseName,Semester,Cyear,CreatedDateTime,Deleted,DeletedDateTime")] Course course)
        {
            _coursesCommandInvoker.SetCoursesCommand(new EditCourseCommand(_context, course, id));
            Course result = await _coursesCommandInvoker.ExecuteCoursesCommand();
            if (result != null)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["message"] = "NoCourseToEdit";
                return NotFound();
            }

        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _coursesCommandInvoker.SetCoursesCommand(new DeleteCourseCommand(_context, id));
            Course result = await _coursesCommandInvoker.ExecuteCoursesCommand();
            if (result != null)
            {
                TempData["message"] = "CourseDeleted";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["message"] = "CourseNotFound";
                return Problem("Entity set 'IDENTITYUSERContext.Courses' is null.");
            }
        }

        private bool CourseExists(int id)
        {
            return (_context.Courses?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> FindCourse(int id)
        {
            var response = await _context.Courses.FindAsync(id);
            return Json(response);
        }
    }
}
