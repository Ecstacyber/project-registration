using ProjectRegistration.Models;
using ProjectRegistration.Decorator.Interfaces;

namespace ProjectRegistration.Decorator
{
    public class MemberCountVerificationDecorator : IProjectVerification
    {
        private readonly IProjectVerification _baseVerification;
        private readonly int _maxMember;

        public MemberCountVerificationDecorator(IProjectVerification baseVerification, int maxMember)
        {
            _baseVerification = baseVerification;
            _maxMember = maxMember;
        }

        public bool VerifyProject(Project project)
        {
            // Perform additional verification based on member count
            var baseVerificationResult = _baseVerification.VerifyProject(project);

            if (baseVerificationResult)
            {
                var memberCount = project.ProjectMembers.Count(x => x.Deleted == false);
                return memberCount <= _maxMember;
            }

            return false;
        }
    }
}
