using ProjectRegistration.Models;
using ProjectRegistration.Decorator.Interfaces;

namespace ProjectRegistration.Decorator
{
    public class DefaultVerification : IProjectVerification
    {
        public bool VerifyProject(Project project)
        {
            return true; 
        }
    }
}
