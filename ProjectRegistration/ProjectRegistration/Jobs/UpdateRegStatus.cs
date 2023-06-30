using Quartz;
using ProjectRegistration.Models;

namespace ProjectRegistration.Jobs
{
    public class UpdateRegStatus : IJob
    {
        private readonly IDENTITYUSERContext _context;

        public UpdateRegStatus(IDENTITYUSERContext context)
        {
            _context = context;
        }

        public Task Execute(IJobExecutionContext context)
        {
            // Note: This method must always return a value 
            // This is especially important for trigger listers watching job execution 
            List<Class> startClasses = _context.Classes.Where(x => x.RegStart >= DateTime.Now && x.RegEnd <= DateTime.Now && x.RegOpen == "Đóng").ToList();
            foreach (Class cls in startClasses)
            {
                cls.RegOpen = "Mở";
            }
            List<Class> endClasses = _context.Classes.Where(x => x.RegEnd >= DateTime.Now && x.RegOpen == "Mở").ToList();
            foreach (Class cls in endClasses)
            {
                cls.RegOpen = "Đóng";
            }
            _context.SaveChanges();
            return Task.FromResult(true);
        }
    }
}
