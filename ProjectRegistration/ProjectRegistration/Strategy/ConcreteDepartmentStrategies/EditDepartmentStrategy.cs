using Microsoft.EntityFrameworkCore;
using ProjectRegistration.Models;
using ProjectRegistration.Strategy.Interface;

namespace ProjectRegistration.Strategy.ConcreteDepartmentStrategies
{
    public class EditDepartmentStrategy : IDepartmentStrategy
    {
        private readonly Department Department;
        private readonly IDENTITYUSERContext _context;

        public EditDepartmentStrategy(Department department, IDENTITYUSERContext context)
        {
            Department = department;
            _context = context;
        }

        public async Task<Department> Execute()
        {
            try
            {
                var result = _context.Update(Department);
                await _context.SaveChangesAsync();
                return result.Entity;
            }
            catch (DbUpdateConcurrencyException)
            {
                return null;
            }
        }
    }
}
