using Microsoft.EntityFrameworkCore;
using ProjectRegistration.Models;
using ProjectRegistration.Strategy.Interface;

namespace ProjectRegistration.Strategy.ConcreteDepartmentStrategies
{
    public class DepartmentDetailsStrategy : IDepartmentStrategy
    {
        private readonly IDENTITYUSERContext _context;
        private readonly int? Id;

        public DepartmentDetailsStrategy(IDENTITYUSERContext context, int? id)
        {
            _context = context;
            Id = id;
        }

        public async Task<Department> Execute()
        {
            if (Id == null)
            {
                return null;
            }
            Department department = await _context.Departments.FirstOrDefaultAsync(m => m.Id == Id);
            if (department == null)
            {
                return null;
            }
            return department;
        }
    }
}
