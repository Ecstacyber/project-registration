using Microsoft.EntityFrameworkCore;
using ProjectRegistration.Models;
using ProjectRegistration.Strategy.Interface;

namespace ProjectRegistration.Strategy.ConcreteDepartmentStrategies
{
    public class DeleteDepartmentStrategy : IDepartmentStrategy
    {
        private readonly int? Id;
        private readonly IDENTITYUSERContext _context;

        public DeleteDepartmentStrategy(int? id, IDENTITYUSERContext context)
        {
            Id = id;
            _context = context;
        }

        public async Task<Department> Execute()
        {
            if (_context.Departments == null || Id == null)
            {
                return null;
            }
            var department = await _context.Departments.FindAsync(Id);
            if (department != null)
            {
                department.Deleted = true;
                department.DeletedDateTime = DateTime.Now;
                await _context.SaveChangesAsync();
                return department;
            }
            return null;
        }
    }
}
