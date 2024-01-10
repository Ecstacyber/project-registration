using Microsoft.EntityFrameworkCore;
using ProjectRegistration.Command.CoursesCommand.Interface;
using ProjectRegistration.Models;

namespace ProjectRegistration.Command.ConcreteCommands
{
    public class AddCourseCommand : ICoursesCommand
    {
        private readonly IDENTITYUSERContext _context;
        private readonly Course _course;

        public AddCourseCommand(IDENTITYUSERContext context, Course course)
        {
            _context = context;
            _course = course;
        }

        public async Task<Course> ExecuteAsync()
        {
            if (_context.Courses.Where(x => x.Deleted == false && x.CourseId == _course.CourseId).FirstOrDefault() != null)
            {
                return null;
            }
            else
            {
                var result = await _context.AddAsync(_course);
                await _context.SaveChangesAsync();
                return result.Entity;
            }
        }
    }
}
