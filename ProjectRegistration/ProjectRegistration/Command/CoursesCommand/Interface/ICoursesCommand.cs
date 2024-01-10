using ProjectRegistration.Models;

namespace ProjectRegistration.Command.CoursesCommand.Interface
{
    public interface ICoursesCommand
    {
        Task<Course> ExecuteAsync();
    }
}
