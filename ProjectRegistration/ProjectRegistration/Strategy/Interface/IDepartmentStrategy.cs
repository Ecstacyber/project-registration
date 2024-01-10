using ProjectRegistration.Models;

namespace ProjectRegistration.Strategy.Interface
{
    public interface IDepartmentStrategy
    {
        public Task<Department> Execute();
    }
}
