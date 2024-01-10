using ProjectRegistration.Command.CoursesCommand.Interface;
using ProjectRegistration.Models;

namespace ProjectRegistration.Command.ConcreteCommands
{
    public class DeleteCourseCommand : ICoursesCommand
    {
        private readonly IDENTITYUSERContext _context;
        private readonly int _id;

        public DeleteCourseCommand(IDENTITYUSERContext context, int id)
        {
            _context = context;
            _id = id;
        }

        public async Task<Course> ExecuteAsync()
        {
            var course = await _context.Courses.FindAsync(_id);
            if (course != null)
            {
                course.Deleted = true;
                course.DeletedDateTime = DateTime.Now;
                await _context.SaveChangesAsync();
                return course;
            }
            else
            {
                return null;
            }
        }
    }
}
