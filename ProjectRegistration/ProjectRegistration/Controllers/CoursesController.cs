﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectRegistration.Models;

namespace ProjectRegistration.Controllers
{
    public class CoursesController : Controller
    {
        private readonly IDENTITYUSERContext _context;

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
                          Problem("Entity set 'IDENTITYUSERContext.Courses'  is null.");
        }

        // GET: Courses/Details/5
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Courses == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
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
            if (_context.Courses.Where(x => x.Deleted == false && x.CourseId == course.CourseId).FirstOrDefault() != null)
            {
                ModelState.AddModelError("CourseId", "Mã môn học đã tồn tại");
            }
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                TempData["message"] = "CourseCreated";
                return RedirectToAction(nameof(Index));
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

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CourseId,CourseName,Semester,Cyear,CreatedDateTime,Deleted,DeletedDateTime")] Course course)
        {
            if (id != course.Id)
            {
                TempData["message"] = "NoCourseToEdit";
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                    TempData["message"] = "CourseCreated";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Id))
                    {
                        TempData["message"] = "NoCourseToEdit";
                        return NotFound();
                    }
                    else
                    {
                        TempData["message"] = "CourseNotCreated";
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            TempData["message"] = "CourseNotCreated";
            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Courses == null)
            {
                TempData["message"] = "CourseNotFound";
                return Problem("Entity set 'IDENTITYUSERContext.Courses'  is null.");
            }
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                course.Deleted = true;
                course.DeletedDateTime = DateTime.Now;
                TempData["message"] = "CourseDeleted";
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
