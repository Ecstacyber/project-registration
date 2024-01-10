using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using ProjectRegistration.Models;

namespace ProjectRegistration.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly IDENTITYUSERContext _context;

        public DepartmentsController(IDENTITYUSERContext context)
        {
            _context = context;
        }

        // GET: Departments

        [Authorize(Roles = "Manager, Lecturer, Student")]
        public async Task<IActionResult> Index()
        {
            return _context.Departments != null ?
                        View(await _context.Departments.Where(x => x.Deleted == false).ToListAsync()) :
                        Problem("Entity set 'IDENTITYUSERContext.Departments'  is null.");
        }

        // GET: Departments/Details/5

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Departments == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // GET: Departments/Create

        [Authorize(Roles = "Manager")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Manager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Dname,Info,CreatedDateTime,Deleted,DeletedDateTime")] Department department)
        {
            if (_context.Departments.Where(x => x.Deleted == false && x.Dname == department.Dname).FirstOrDefault() != null)
            {
                ModelState.AddModelError("Dname", "Mã khoa đã tồn tại");
            }
            if (ModelState.IsValid)
            {
                department.CreatedDateTime = DateTime.Now;
                department.Deleted = false;
                _context.Add(department);
                await _context.SaveChangesAsync();
                TempData["message"] = "DepartmentCreated";
                return RedirectToAction(nameof(Index));
            }
            TempData["message"] = "DepartmentNotCreated";
            return View(department);
        }

        // GET: Departments/Edit/5
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Departments == null)
            {
                return NotFound();
            }

            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            return View(department);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Dname,Info,CreatedDateTime,Deleted,DeletedDateTime")] Department department)
        {
            if (id != department.Id)
            {
                TempData["message"] = "NoDepartmentToEdit";
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(department);
                    await _context.SaveChangesAsync();
                    TempData["message"] = "DepartmentEdited";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(department.Id))
                    {
                        TempData["message"] = "NoDepartmentToEdit";
                        return NotFound();
                    }
                    else
                    {
                        TempData["message"] = "DepartmentNotEdited";
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            TempData["message"] = "Failed";
            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Departments == null)
            {
                TempData["message"] = "DepartmentNotDeleted";
                return Problem("Entity set 'IDENTITYUSERContext.Departments'  is null.");
            }
            var department = await _context.Departments.FindAsync(id);
            if (department != null)
            {
                department.Deleted = true;
                department.DeletedDateTime = DateTime.Now;
                TempData["message"] = "DepartmentDeleted";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DepartmentExists(int id)
        {
            return (_context.Departments?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}