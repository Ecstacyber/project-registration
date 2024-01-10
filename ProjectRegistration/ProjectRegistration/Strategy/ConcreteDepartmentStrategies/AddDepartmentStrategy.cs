using Microsoft.EntityFrameworkCore;
using ProjectRegistration.Models;
using ProjectRegistration.Strategy.Interface;

namespace ProjectRegistration.Strategy.ConcreteDepartmentStrategies
{
    public class AddDepartmentStrategy : IDepartmentStrategy
    {
        private readonly Department Department;
        private readonly IDENTITYUSERContext _context;

        public AddDepartmentStrategy(Department department, IDENTITYUSERContext context)
        {
            Department = department;
            _context = context;
        }

        public async Task<Department> Execute()
        {
            Department.CreatedDateTime = DateTime.Now;
            Department.Deleted = false;
            var result = await _context.AddAsync(Department);
            await _context.SaveChangesAsync();
            return result.Entity;
        }
    }
}
