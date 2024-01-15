using ProjectRegistration.Models;
using ProjectRegistration.Decorator.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ProjectRegistration.Decorator
{
    public class UniqueUserVerificationDecorator : IProjectVerification
    {
        private readonly IProjectVerification _baseVerification;
        private readonly IDENTITYUSERContext _context;

        public UniqueUserVerificationDecorator(IProjectVerification baseVerification, IDENTITYUSERContext context)
        {
            _baseVerification = baseVerification;
            _context = context;
        }

        public bool VerifyProject(Project project)
        {
            var baseVerificationResult = _baseVerification.VerifyProject(project);

            if (baseVerificationResult)
            {
                foreach (var user in project.ProjectMembers)
                {
                    if (_context.ProjectMembers.Include(x => x.Project).Any(x => x.Project.ClassId == project.ClassId 
                    && x.StudentId == user.StudentId 
                    && x.ProjectId != project.Id
                    && x.Deleted == false))
                        return false;
                }
                return true;
            }
            return false;
        }
    }
}
