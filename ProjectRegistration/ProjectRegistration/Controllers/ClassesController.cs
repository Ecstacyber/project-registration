using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Text.Json;
using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using ProjectRegistration.Models;
using NuGet.Versioning;
using Microsoft.AspNetCore.Mvc.Formatters;
using Quartz;
using Microsoft.AspNetCore.Identity;

namespace ProjectRegistration.Controllers
{
    public class ClassesController : Controller
    {
        private readonly IDENTITYUSERContext _context;
        private readonly ILogger<ClassesController> _logger;
        private readonly UserManager<User> _userManager;

        public ClassesController(IDENTITYUSERContext context, ILogger<ClassesController> logger, UserManager<User> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        // GET: Classes
        [Authorize(Roles = "Manager, Lecturer, Student")]
        public async Task<IActionResult> Index()
        {
            var IDENTITYUSERContext = _context.Classes.Include(x => x.Course).Where(x => x.Deleted == false).OrderBy(x => x.Id).Reverse();
            return View(await IDENTITYUSERContext.ToListAsync());
        }
        public async Task<IActionResult> SE121()
        {
            var IDENTITYUSERContext = _context.Classes.Include(x => x.Course).Where(x => x.Deleted == false && x.Course.CourseId == "SE121").OrderBy(x => x.Cyear).Reverse();
            ViewData["CourseId"] = "SE121";
            return View("Index", await IDENTITYUSERContext.ToListAsync());
        }
        public async Task<IActionResult> SE122()
        {
            var IDENTITYUSERContext = _context.Classes.Include(x => x.Course).Where(x => x.Deleted == false && x.Course.CourseId == "SE122").OrderBy(x => x.Cyear).Reverse();
            ViewData["CourseId"] = "SE122";
            return View("Index", await IDENTITYUSERContext.ToListAsync());
        }
        public async Task<IActionResult> KLTN()
        {
            var IDENTITYUSERContext = _context.Classes.Include(x => x.Course).Where(x => x.Deleted == false && x.Course.CourseId == "KLTN").OrderBy(x => x.Cyear).Reverse();
            ViewData["CourseId"] = "KLTN";
            return View("Index", await IDENTITYUSERContext.ToListAsync());
        }

        // GET: Classes/Details/5
        [Authorize(Roles = "Manager, Lecturer, Student")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Classes == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes
                .Where(x => x.Deleted == false)
                .Include(x => x.Course)
                .Include(x => x.ProjectClasses)
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
        [Authorize(Roles = "Manager")]
        public IActionResult Create(string? id)
        {
            Class @class = new Class();
            if (id != null)
            {
                @class.CourseId = _context.Courses.Where(x => x.CourseId == id).FirstOrDefault().Id;
            }

            int currentYear = DateTime.Now.Year;
            var yearList = new List<string>();
            for (int i = 2000; i <= currentYear; i++)
            {
                yearList.Add(i.ToString() + " - " + (i + 1).ToString());
            }
            yearList.Reverse();
            ViewData["Year"] = new SelectList(yearList);
            ViewData["Course"] = new SelectList(_context.Courses.Where(x => x.Deleted == false), "Id", "CourseName");
            return View(@class);
        }

        // POST: Classes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Create([Bind("CourseId,ClassId,Semester,Cyear,RegOpen,RegStart")] Class @class, string RegTime)
        {
            if (@class.ClassId.Split('.')[1] == "")
            {
                ModelState.AddModelError("ClassId", "Vui lòng nhập mã lớp");
            }

            if (ModelState.IsValid)
            {
                @class.RegStart = DateTime.Parse(RegTime.Split("-")[0]);
                @class.RegEnd = DateTime.Parse(RegTime.Split('-')[1]);
                @class.CreatedDateTime = DateTime.Now;
                _context.Add(@class);
                await _context.SaveChangesAsync();
                TempData["message"] = "ClassCreated";
                return RedirectToAction(nameof(Index));
            }
            int currentYear = DateTime.Now.Year;
            var yearList = new List<string>();
            for (int i = 2000; i <= currentYear; i++)
            {
                yearList.Add(i.ToString() + " - " + (i + 1).ToString());
            }
            yearList.Reverse();
            ViewData["Year"] = new SelectList(yearList);
            ViewData["Course"] = new SelectList(_context.Courses.Where(x => x.Deleted == false), "Id", "CourseName", @class.CourseId);
            TempData["message"] = "ClassNotCreated";
            return View(@class);
        }

        // GET: Classes/Edit/5
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Classes == null)
            {
                TempData["message"] = "NoClassToEdit";
                return NotFound();
            }

            var @class = await _context.Classes.Include(x => x.Course).Where(x => x.Id == id && x.Deleted == false).FirstOrDefaultAsync();
            if (@class == null)
            {
                TempData["message"] = "NoClassToEdit";
                return NotFound();
            }

            int currentYear = DateTime.Now.Year;
            var yearList = new List<string>();
            for (int i = 2000; i <= currentYear; i++)
            {
                yearList.Add(i.ToString() + " - " + (i + 1).ToString());
            }
            yearList.Reverse();
            ViewData["Course"] = new SelectList(_context.Courses.Where(x => x.Deleted == false), "Id", "CourseName", @class.CourseId);
            ViewData["Year"] = new SelectList(yearList);

            ViewData["RegTime"] = @class.RegStart.Value + " - " + @class.RegEnd.Value;
            return View(@class);
        }

        // POST: Classes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CourseId,ClassId,Semester,Cyear,RegOpen,RegStart")] Class @class, string RegTime)
        {
            if (id != @class.Id)
            {
                TempData["message"] = "NoClassToEdit";
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    @class.RegStart = DateTime.Parse(RegTime.Split("-")[0]);
                    @class.RegEnd = DateTime.Parse(RegTime.Split('-')[1]);
                    _context.Update(@class);
                    await _context.SaveChangesAsync();
                    TempData["message"] = "ClassEdited";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassExists(@class.Id))
                    {
                        TempData["message"] = "NoClassToEdit";
                        return NotFound();
                    }
                    else
                    {
                        TempData["message"] = "ClassNotEdited";
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            int currentYear = DateTime.Now.Year;
            var yearList = new List<string>();
            for (int i = 2000; i <= currentYear; i++)
            {
                yearList.Add(i.ToString() + " - " + (i + 1).ToString());
            }
            yearList.Reverse();
            ViewData["Year"] = new SelectList(yearList);
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "CourseId", @class.CourseId);
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
        [Authorize(Roles = "Manager")]
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
                TempData["message"] = "ClassDeleted";
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
        [Authorize(Roles = "Manager")]
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
                    var @class = _context.Classes.Where(x => x.Deleted == false && x.Id == Id).FirstOrDefault();
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
            return RedirectToAction("Details", new { id = Id });
        }

        private class UserTemp
        {
            public int stt;
            public string Id { get; set; }
            public string UserId { get; set; }
            public string Name { get; set; }
        }
        [HttpGet]
        [Authorize(Roles = "Manager, Lecturer")]
        public async Task<IActionResult> GetUsersNotInClass(string classId, string userId)
        {
            if (userId == null)
            {
                return Json(new List<UserTemp>());
            }
            var allusers = _context.Users.Include(x => x.ClassDetails).Where(x => x.Deleted == false && x.UserId.Contains(userId));
            var classDetails = _context.ClassDetails.Include(x => x.User).Where(x => x.ClassId == int.Parse(classId)).ToList();
            var userList = new List<UserTemp>();
            foreach (var item in allusers)
            {
                if (classDetails.Where(x => x.User.Id == item.Id).FirstOrDefault() == null)
                {
                    var temp = new UserTemp();
                    temp.Id = item.UserId;
                    temp.UserId = item.UserId;
                    temp.Name = item.Fullname;
                    userList.Add(temp);
                }
            }
            return Json(userList);
        }

        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (_context.Classes == null)
            {
                return Problem("Entity set 'ProjectRegistrationManagementContext.Classes'  is null.");
            }
            string[] ids = id.Split('-');
            var @classDetails = _context.ClassDetails.Where(x => x.UserId == ids[0] && x.ClassId == int.Parse(ids[1])).FirstOrDefault();
            if (@classDetails != null)
            {
                @classDetails.Deleted = true;
                @classDetails.DeletedDateTime = DateTime.Now;
                TempData["message"] = "UserDeleted";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = int.Parse(ids[1]) });
        }

        [HttpPost, ActionName("AddUser")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager, Lecturer")]
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
                TempData["message"] = "UserAdded";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = int.Parse(ids[1]) });
        }

        [ActionName("ViewProjectList")]
        [Authorize(Roles = "Manager, Lecturer, Student")]
        public async Task<IActionResult> ViewProjectList(int id)
        {
            var @class = _context.Classes.Where(x => x.Deleted == false && x.Id == id)
                .Include(x => x.ProjectClasses).ThenInclude(x => x.GuidingLecturer)
                .Include(x => x.ProjectClasses).ThenInclude(x => x.GradingLecturer)
                .Include(x => x.ProjectClasses).ThenInclude(x => x.ProjectMembers).ThenInclude(x => x.Student)
                .FirstOrDefault();
            var projectList = new List<Project>();
            projectList = @class.ProjectClasses.Where(x => x.Deleted == false && x.State != "Chưa chấp thuận").ToList();
            ViewData["ClassId"] = id;
            return View(projectList);
        }

        // GET: Projects/Create
        [Authorize(Roles = "Manager, Lecturer")]
        public IActionResult CreateProject(int id)
        {
            var leturerList = _context.ClassDetails.Include(x => x.User).Where(x => x.ClassId == id && x.Deleted == false && x.User.Deleted == false && x.User.UserTypeId == 10).ToList();
            ViewData["GradingLecturerId"] = new SelectList(leturerList, "User.Id", "User.Fullname");
            ViewData["GuidingLecturerId"] = new SelectList(leturerList, "User.Id", "User.Fullname");

            ViewData["id"] = id;
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Id");
            ViewData["ClassId2"] = new SelectList(_context.Classes, "Id", "Id");
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Id");
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("CreateProject")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager, Lecturer")]
        public async Task<IActionResult> CreateProject([Bind("Pname,Info,DepartmentId,ClassId,ClassId2,GuidingLecturerId,GradingLecturerId,Pyear,Semester,CreatedDateTime,Deleted,DeletedDateTime")] Project project)
        {
            if (ModelState.IsValid)
            {
                var currentClass = _context.Classes.FirstOrDefault(x => x.Id == project.ClassId);
                if (currentClass != null && currentClass.Course != null)
                {
                    if (currentClass.Course.CourseName == "Đồ án 1" || currentClass.Course.CourseName == "Đồ án 2")
                    {
                        if (currentClass.Semester != null && currentClass.Cyear != null && project.GradingLecturerId != null && project.GuidingLecturerId != null)
                        {
                            var pj1 = _context.Courses.FirstOrDefault(x => x.CourseId == "SE121");
                            var pj2 = _context.Courses.FirstOrDefault(x => x.CourseId == "SE122");
                            var projectsInCurrentSem = _context.Projects.Where(x => x.Class.Semester == currentClass.Semester
                                                                                && x.Class.Cyear == currentClass.Cyear
                                                                                && x.Class.CourseId == pj1.Id
                                                                                && x.Class.CourseId == pj2.Id).ToList();
                            var guidingLecturer = _context.Users.FirstOrDefault(x => x.Id == project.GuidingLecturerId);
                            var guLecProjects = projectsInCurrentSem.Where(x => x.GuidingLecturerId == guidingLecturer.Id);
                            if (guLecProjects.Count() > 20)
                            {
                                TempData["message"] = "Project1GuidingLimitReached";
                                TempData["limit"] = 20;
                                return View(project);
                            }
                        }
                    }
                    if (currentClass.Course.CourseName == "Khóa luận")
                    {
                        var projectsInCurrentSem = _context.Projects.Where(x => x.Class.Semester == currentClass.Semester && x.Class.Cyear == currentClass.Cyear).ToList();
                        var guidingLecturer = _context.Users.FirstOrDefault(x => x.Id == project.GuidingLecturerId);
                        var guLecProjects = projectsInCurrentSem.Where(x => x.GuidingLecturerId == guidingLecturer.Id);
                        if (guLecProjects.Count() > 5)
                        {
                            TempData["message"] = "GraduationThesisGuidingLimitReached";
                            TempData["limit"] = 5;
                            return View(project);
                        }
                    }
                }

                project.CreatedDateTime = DateTime.Now; 
                ClaimsPrincipal currentUser = this.User;
                var currentUserName = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = await _userManager.FindByIdAsync(currentUserName);
                project.State = "Chưa chấp thuận";
                _context.Add(project);
                await _context.SaveChangesAsync();
                TempData["message"] = "ProjectCreated";
                return RedirectToAction("ViewProjectList", new { id = project.ClassId });
            }
            ViewData["result"] = "Failed";
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Id", project.ClassId);
            ViewData["ClassId2"] = new SelectList(_context.Classes, "Id", "Id", project.ClassId2);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Id", project.DepartmentId);
            var leturerList = _context.ClassDetails.Include(x => x.User).Where(x => x.ClassId == project.ClassId && x.Deleted == false && x.User.Deleted == false && x.User.UserTypeId == 10).ToList();
            ViewData["GradingLecturerId"] = new SelectList(leturerList, "User.Id", "User.Fullname");
            ViewData["GuidingLecturerId"] = new SelectList(leturerList, "User.Id", "User.Fullname");
            TempData["message"] = "ProjectNotCreated";
            return RedirectToAction("CreateProject", new { id = project.ClassId });
        }

        [HttpPost, ActionName("AddProjectFromFileExcel")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> AddProjectFromFileExcel(IFormCollection form, IFormFile fileSelect)
        {
            try
            {
                int classId = int.Parse(form["classId"]);
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
                        reader.Read();
                        reader.Read();
                        var @class = _context.Classes.Where(x => (x.Deleted == false && x.Id == classId)).FirstOrDefault();

                        var cd = new List<Project>();
                        foreach (var x in @class.ProjectClasses)
                        {
                            cd.Add(x);
                        }

                        while (reader.Read()) //Each row of the file
                        {
                            if (reader.GetValue(1) == null) continue;
                            var existProject = _context.Projects
                                .Where(x => x.Deleted == false && x.Pname == reader.GetValue(1).ToString() && x.ClassId == classId).FirstOrDefault();
                            if (existProject != null) continue;
                            var project = new Project();
                            project.Pname = reader.GetValue(1).ToString();
                            var user = _context.Users.Where(x => (x.Fullname == reader.GetValue(3).ToString() && x.Deleted == false)).FirstOrDefault();
                            if (user == null) continue;
                            project.GuidingLecturer = user;
                            project.GuidingLecturerId = user.Id;
                            project.Info = reader.GetValue(2) == null ? "" : reader.GetValue(2).ToString();
                            project.CreatedDateTime = DateTime.Now;
                            project.ClassId = classId;
                            project.State = "Chưa đăng ký";
                            cd.Add(project);
                            _context.Projects.Add(project);
                        }
                        @class.ProjectClasses = cd;
                    }
                }

                System.IO.File.Delete(filepath);

                await _context.SaveChangesAsync();
                TempData["message"] = "ProjectCreated";
                return RedirectToAction("ViewProjectList", new { id = classId });
            }
            catch
            {
                int classId = int.Parse(form["classId"]);
                TempData["message"] = "ProjectNotCreated";
                return RedirectToAction("ViewProjectList", new { id = classId });
            }
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("DeleteProject")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager, Lecturer")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            if (_context.Projects == null)
            {
                TempData["message"] = "NoProjectToDelete";
                return Problem("Entity set 'ProjectRegistrationManagementContext.Projects'  is null.");
            }
            var project = await _context.Projects.FindAsync(id);
            if (project != null)
            {
                project.Deleted = true;
                project.DeletedDateTime = DateTime.Now;
                TempData["message"] = "ProjectDeleted";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("ViewProjectList", new { id = project.ClassId });
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Manager, Lecturer, Student")]
        public async Task<IActionResult> EditProject(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            ViewData["id"] = project.ClassId;
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Id", project.ClassId);
            ViewData["ClassId2"] = new SelectList(_context.Classes, "Id", "Id", project.ClassId2);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Id", project.DepartmentId);
            var leturerList = _context.ClassDetails.Include(x => x.User).Where(x => x.ClassId == id && x.Deleted == false && x.User.Deleted == false && x.User.UserTypeId == 10).ToList();
            ViewData["GradingLecturerId"] = new SelectList(leturerList, "User.Id", "User.Fullname");
            ViewData["GuidingLecturerId"] = new SelectList(leturerList, "User.Id", "User.Fullname");
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("EditProject")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager, Lecturer, Student")]
        public async Task<IActionResult> EditProject(int id, [Bind("Id,Pname,Info,DepartmentId,ClassId,ClassId2,GuidingLecturerId,GradingLecturerId,Pyear,Semester,CreatedDateTime,Deleted,DeletedDateTime")] Project project)
        {
            if (id != project.Id)
            {
                TempData["message"] = "NoProjectToEdit";
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var currentClass = _context.Classes.FirstOrDefault(x => x.Id == project.ClassId);
                if (currentClass != null && currentClass.Course != null)
                {
                    if (currentClass.Course.CourseName == "Đồ án 1")
                    {
                        if (currentClass.Semester != null && currentClass.Cyear != null && project.GradingLecturerId != null && project.GuidingLecturerId != null)
                        {
                            var projectsInCurrentSem = _context.Projects.Where(x => x.Class.Semester == currentClass.Semester && x.Class.Cyear == currentClass.Cyear).ToList();
                            var guidingLecturer = _context.Users.FirstOrDefault(x => x.Id == project.GuidingLecturerId);
                            var guLecProjects = projectsInCurrentSem.Where(x => x.GuidingLecturerId == guidingLecturer.Id);
                            if (guLecProjects.Count() > 20)
                            {
                                TempData["message"] = "Project1GuidingLimitReached";
                                return View(project);
                            }
                        }
                    }
                    if (currentClass.Course.CourseName == "Khóa luận")
                    {
                        var projectsInCurrentSem = _context.Projects.Where(x => x.Class.Semester == currentClass.Semester && x.Class.Cyear == currentClass.Cyear).ToList();
                        var guidingLecturer = _context.Users.FirstOrDefault(x => x.Id == project.GuidingLecturerId);
                        var guLecProjects = projectsInCurrentSem.Where(x => x.GuidingLecturerId == guidingLecturer.Id);
                        if (guLecProjects.Count() > 5)
                        {
                            TempData["message"] = "GraduationThesisGuidingLimitReached";
                            return View(project);
                        }
                    }
                }
                try
                {
                    if (project.PGrade >= 5)
                        project.State = "Đã hoàn thành";
                    if (project.PGrade < 5)
                        project.State = "Rớt";

                    _context.Update(project);
                    await _context.SaveChangesAsync();
                    TempData["message"] = "ProjectEdited";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Projects.Any(x => x.Id == project.Id))
                    {
                        TempData["message"] = "NoProjectToEdit";
                        return NotFound();
                    }
                    else
                    {
                        TempData["message"] = "ProjectNotEdited";
                        throw;
                    }
                }
                return RedirectToAction("ViewProjectList", new { id = project.ClassId });
            }
            ViewData["result"] = "Failed";
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Id", project.ClassId);
            ViewData["ClassId2"] = new SelectList(_context.Classes, "Id", "Id", project.ClassId2);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Id", project.DepartmentId);
            var leturerList = _context.ClassDetails.Include(x => x.User).Where(x => x.ClassId == id && x.Deleted == false && x.User.Deleted == false && x.User.UserTypeId == 10).ToList();
            ViewData["GradingLecturerId"] = new SelectList(leturerList, "User.Id", "User.Fullname");
            ViewData["GuidingLecturerId"] = new SelectList(leturerList, "User.Id", "User.Fullname");
            TempData["message"] = "ProjectNotEdited";
            return View(project);
        }

        // GET: Projects/Details/5
        [Authorize(Roles = "Manager, Lecturer, Student")]
        public async Task<IActionResult> ProjectDetails(string? id)
        {
            int pId = int.Parse(id.Split('-')[0]);
            int cId = int.Parse(id.Split('-')[1]);
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Where(p => p.Deleted == false)
                .Include(p => p.Class)
                .Include(p => p.ProjectMembers)
                .ThenInclude(p => p.Student)
                .Include(p => p.GradingLecturer)
                .Include(p => p.GuidingLecturer)
                .Include(p => p.Products)
                .FirstOrDefaultAsync(m => m.Id == pId);
            if (project == null)
            {
                return NotFound();
            }
            ViewData["cId"] = cId;

            return View(project);
        }

        [Authorize(Roles = "Manager, Lecturer, Student")]
        public IActionResult AddMemberToProject(string id)
        {
            int pId = int.Parse(id.Split('-')[0]);
            int cId = int.Parse(id.Split('-')[1]);
            var project = _context.Projects.FirstOrDefault(x => x.Id == pId);
            ViewData["ClassId"] = cId;
            ViewData["Project"] = project;
            ViewData["ProjectId"] = project.Id;
            //var members = _context.ClassDetails.Where(x => x.ClassId == project.ClassId).Include(x => x.User).ToList();
            //if (members != null)
            //{
            //    var classDetail1 = _context.ClassDetails.Where(x => x.ClassId == project.ClassId).Include(x => x.User).ToList();
            //    ViewData["StudentId1"] = new SelectList(classDetail1, "UserId", "User.Fullname");
            //    ViewData["StudentId2"] = new SelectList(members, "UserId", "User.Fullname");
            //}
            var classDetails = _context.ClassDetails.Include(x => x.User).Where(x => x.ClassId == project.ClassId && x.User.UserTypeId == 100).ToList();
            ViewData["StudentId1"] = new SelectList(classDetails, "UserId", "User.Fullname" + ' ' + "User.UserId");
            ViewData["StudentId2"] = new SelectList(classDetails, "UserId", "User.Fullname" + ' ' + "User.UserId");
            return View();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager, Lecturer, Student")]
        public async Task<IActionResult> AddMemberToProject([Bind("ProjectId, GroupName")] ProjectMember projectMember, string StudentId1, string StudentId2)
        {
            ModelState.Remove("StudentId2");
            Project saidProject = new();
            if (ModelState.IsValid)
            {
                saidProject = _context.Projects.FirstOrDefault(x => x.Id == projectMember.ProjectId);
                
                var regStart = saidProject.Class.RegStart;
                var regEnd = saidProject.Class.RegEnd;
                if (saidProject != null)
                {
                    var currentClass = _context.Classes.FirstOrDefault(x => x.Id == saidProject.ClassId);
                    var projectsInCurrentClass = _context.Projects.Where(x => x.ClassId == currentClass.Id).ToList();
                    if (projectMember != null && regStart != null && regStart <= DateTime.Now)
                    {
                        TempData["message"] = "RegNotOpened";
                        return RedirectToAction("ProjectDetails", new { id = saidProject.Id });
                    }
                    if (projectMember != null && regEnd != null && regEnd >= DateTime.Now)
                    {
                        TempData["message"] = "RegClosed";
                        return RedirectToAction("ProjectDetails", new { id = saidProject.Id });
                    }
                    if (saidProject.ProjectMembers.Count == 2)
                    {
                        TempData["message"] = "MemberLimitReached";
                        return RedirectToAction("ProjectDetails", new { id = saidProject.Id });
                    }
                    foreach (Project pj in projectsInCurrentClass)
                    {
                        if (pj.ProjectMembers.Any(x => x.StudentId == StudentId1 && x.Deleted == false))
                        {
                            TempData["message"] = "Student1HadProject";
                            return RedirectToAction("ProjectDetails", new { id = saidProject.Id });
                        }
                    }
                    foreach (Project pj in projectsInCurrentClass)
                    {
                        if (pj.ProjectMembers.Any(x => x.StudentId == StudentId2 && x.Deleted == false))
                        {
                            TempData["message"] = "Student2HadProject";
                            return RedirectToAction("ProjectDetails", new { id = saidProject.Id });
                        }
                    }
                    var projectMember1 = new ProjectMember
                    {
                        CreatedDateTime = DateTime.Now
                    };
                    projectMember1.Project = _context.Projects.FirstOrDefault(x => x.Id == projectMember1.ProjectId);
                    projectMember1.ProjectId = projectMember.ProjectId;
                    projectMember1.Student = _context.Users.FirstOrDefault(x => x.Id == projectMember1.StudentId);
                    projectMember1.StudentId = StudentId1;
                    projectMember1.GroupName = projectMember.GroupName;
                    _context.Add(projectMember1);
                    if (StudentId2 != null)
                    {
                        var projectMember2 = new ProjectMember
                        {
                            CreatedDateTime = DateTime.Now
                        };
                        projectMember2.Project = _context.Projects.FirstOrDefault(x => x.Id == projectMember2.ProjectId);
                        projectMember2.ProjectId = projectMember.ProjectId;
                        projectMember2.Student = _context.Users.FirstOrDefault(x => x.Id == projectMember2.StudentId);
                        projectMember2.StudentId = StudentId2;
                        projectMember2.GroupName = projectMember.GroupName;
                        _context.Add(projectMember2);
                    }
                    saidProject.State = "Đang thực hiện";
                    await _context.SaveChangesAsync();
                    TempData["message"] = "MemberAddedToProject";
                    return RedirectToAction("ProjectDetails", new { id = projectMember1.ProjectId });
                }
            }

            var project = _context.Projects.FirstOrDefault(x => x.Id == projectMember.ProjectId);
            ViewData["Project"] = project;
            var classDetails = _context.ClassDetails.Where(x => x.ClassId == project.ClassId).Include(x => x.User).ToList();

            TempData["message"] = "MemberNotAddedToProject";
            ViewData["StudentId1"] = new SelectList(classDetails, "UserId", "User.Fullname", StudentId1);
            ViewData["StudentId2"] = new SelectList(classDetails, "UserId", "User.Fullname", StudentId2);
            return View(projectMember);
        }

        // POST: Classes/DeleteMemberFromProject/5
        [Authorize(Roles = "Manager, Lecturer, Student")]
        [HttpPost, ActionName("DeleteMemberFromProject")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteMemberFromProject(int id)
        {
            if (_context.ProjectMembers == null)
            {
                TempData["message"] = "NoMemberToDeleteFromProject";
                return Problem("Entity set 'IDENTITYUSERContext.ProjectMembers'  is null.");
            }
            var member = await _context.ProjectMembers.FindAsync(id);
            if (member != null)
            {
                member.Deleted = true;
                member.DeletedDateTime = DateTime.Now;
                TempData["message"] = "MemberDeletedFromProject";               
            }

            await _context.SaveChangesAsync();
            var currentProject = _context.Projects.FirstOrDefault(x => x.Id == member.ProjectId);
            if (currentProject != null && currentProject.ProjectMembers.Where(x => x.Deleted == false).Count() == 0)
            {
                currentProject.State = "Chưa đăng ký";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ProjectDetails", new { id = id });
        }

        // POST
        [HttpPost, ActionName("VerifyProject")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> VerifyProject(int id)
        {
            var project = _context.Projects.FirstOrDefault(x => x.Id == id);
            if (project != null)
            {
                project.State = "Chưa đăng ký";
                _context.SaveChanges();
                TempData["message"] = "ProjectVerified";
            }
            else
            {
                TempData["message"] = "CouldNotVerifyProject";
            }
            return RedirectToAction("Details", new { id = project.ClassId });
        }

        private class UnverifyProjectDetail
        {
            public string Pname { get; set; }
            public string? Info { get; set; }
            public string GuidingLecturerId { get; set; }
            public string GuidingLecturerName { get; set; }

        }
        [Authorize(Roles = "Manager, Lecturer, Student")]
        public async Task<IActionResult> GetProjectDetail(int id)
        {
            var pj = await _context.Projects.Include(x => x.GuidingLecturer).Where(x => x.Deleted == false && x.Id == id).FirstOrDefaultAsync();
            var response = new UnverifyProjectDetail();
            response.Pname = pj.Pname;
            response.Info = pj.Info;
            response.GuidingLecturerId = pj.GuidingLecturerId;
            response.GuidingLecturerName = pj.GuidingLecturer.Fullname;
            return Json(response);
        }
    }
}
