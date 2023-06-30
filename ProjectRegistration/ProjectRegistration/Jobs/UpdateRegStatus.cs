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
            List<Class> startClasses = _context.Classes.Where(x => x.RegStart >= DateTime.Now && x.RegOpen == "Đóng").ToList();
            if (startClasses.Count > 0)
            {
                foreach (Class cls in startClasses)
                {
                    cls.RegOpen = "Mở";
                }
                _context.SaveChanges();
            }
            List<Class> endClasses = _context.Classes.Where(x => x.RegEnd >= DateTime.Now && x.RegOpen == "Mở").ToList();
            if (endClasses.Count > 0)
            {
                foreach (Class cls in endClasses)
                {
                    cls.RegOpen = "Đóng";
                }
                _context.SaveChanges();
            }
            return Task.FromResult(true);
        }
    }
}
