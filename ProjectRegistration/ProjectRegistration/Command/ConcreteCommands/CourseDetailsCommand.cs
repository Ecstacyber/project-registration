using Microsoft.EntityFrameworkCore;
using ProjectRegistration.Command.CoursesCommand.Interface;
using ProjectRegistration.Models;

namespace ProjectRegistration.Command.ConcreteCommands
{
    public class CourseDetailsCommand : ICoursesCommand
    {
        private readonly IDENTITYUSERContext _context;
        private readonly int _id;

        public CourseDetailsCommand(IDENTITYUSERContext context, int id)
        {
            _context = context;
            _id = id;
        }

        public async Task<Course> ExecuteAsync()
        {
            return await _context.Courses.FirstOrDefaultAsync(m => m.Id == _id);
        }
    }
}
