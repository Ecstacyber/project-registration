using ProjectRegistration.Models;
using ProjectRegistration.Decorator.Interfaces;

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
                var uniqueUsers = project.ProjectMembers
                    .Where(x => x.Deleted == false)
                    .Select(x => x.StudentId)
                    .Distinct();

                var isUnique = uniqueUsers.Count() == project.ProjectMembers.Count(x => x.Deleted == false);
                return isUnique;
            }

            return false;
        }
    }
}
