using ProjectRegistration.Command.CoursesCommand.Interface;
using ProjectRegistration.Models;

namespace ProjectRegistration.Command.Invoker
{
    public class CoursesCommandInvoker
    {
        private ICoursesCommand _coursesCommand;

        public void SetCoursesCommand(ICoursesCommand coursesCommand)
        {
            _coursesCommand = coursesCommand;
        }

        public async Task<Course> ExecuteCoursesCommand()
        {
            return await _coursesCommand.ExecuteAsync();
        }
    }
}
