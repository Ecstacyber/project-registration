using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectRegistration.Models;

namespace ProjectRegistration.Controllers
{
    public class ClassDetailsController : Controller
    {
        private readonly ProjectRegistrationManagementContext _context;

        public ClassDetailsController(ProjectRegistrationManagementContext context)
        {
            _context = context;
        }

        // GET: ClassDetails
        public async Task<IActionResult> Index()
        {
            var projectRegistrationManagementContext = _context.ClassDetails.Include(c => c.Class).Include(c => c.User);
            return View(await projectRegistrationManagementContext.ToListAsync());
        }

        // GET: ClassDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ClassDetails == null)
            {
                return NotFound();
            }

            var classDetail = await _context.ClassDetails
                .Include(c => c.Class)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (classDetail == null)
            {
                return NotFound();
            }

            return View(classDetail);
        }

        // GET: ClassDetails/Create
        public IActionResult Create()
        {
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: ClassDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClassId,UserId,CreatedDateTime,Deleted,DeletedDateTime")] ClassDetail classDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(classDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Id", classDetail.ClassId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", classDetail.UserId);
            return View(classDetail);
        }

        // GET: ClassDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ClassDetails == null)
            {
                return NotFound();
            }

            var classDetail = await _context.ClassDetails.FindAsync(id);
            if (classDetail == null)
            {
                return NotFound();
            }
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Id", classDetail.ClassId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", classDetail.UserId);
            return View(classDetail);
        }

        // POST: ClassDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClassId,UserId,CreatedDateTime,Deleted,DeletedDateTime")] ClassDetail classDetail)
        {
            if (id != classDetail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(classDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassDetailExists(classDetail.Id))
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
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Id", classDetail.ClassId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", classDetail.UserId);
            return View(classDetail);
        }

        // GET: ClassDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ClassDetails == null)
            {
                return NotFound();
            }

            var classDetail = await _context.ClassDetails
                .Include(c => c.Class)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (classDetail == null)
            {
                return NotFound();
            }

            return View(classDetail);
        }

        // POST: ClassDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ClassDetails == null)
            {
                return Problem("Entity set 'ProjectRegistrationManagementContext.ClassDetails'  is null.");
            }
            var classDetail = await _context.ClassDetails.FindAsync(id);
            if (classDetail != null)
            {
                _context.ClassDetails.Remove(classDetail);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClassDetailExists(int id)
        {
          return (_context.ClassDetails?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
