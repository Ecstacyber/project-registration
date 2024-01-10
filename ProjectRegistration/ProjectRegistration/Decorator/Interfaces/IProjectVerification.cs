using ProjectRegistration.Models;

namespace ProjectRegistration.Decorator.Interfaces
{
    public interface IProjectVerification
    {
        bool VerifyProject(Project project);
    }
}
