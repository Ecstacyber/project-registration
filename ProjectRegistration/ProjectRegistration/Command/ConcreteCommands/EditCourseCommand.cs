using Microsoft.EntityFrameworkCore;
using ProjectRegistration.Command.CoursesCommand.Interface;
using ProjectRegistration.Models;

namespace ProjectRegistration.Command.ConcreteCommands
{
    public class EditCourseCommand : ICoursesCommand
    {
        private readonly IDENTITYUSERContext _context;
        private readonly Course _course;
        private readonly int _id;

        public EditCourseCommand(IDENTITYUSERContext context, Course course, int id)
        {
            _context = context;
            _course = course;
            _id = id;
        }

        public async Task<Course> ExecuteAsync()
        {
            if (_id != _course.Id)
            {
                return null;
            }
            try
            {
                var result = _context.Update(_course);
                await _context.SaveChangesAsync();
                return result.Entity;
            }
            catch (DbUpdateConcurrencyException)
            {
                return null;
            }
        }
    }
}
