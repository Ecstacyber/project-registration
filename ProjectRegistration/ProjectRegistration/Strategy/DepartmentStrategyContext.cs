using ProjectRegistration.Models;
using ProjectRegistration.Strategy.Interface;

namespace ProjectRegistration.Strategy
{
    public class DepartmentStrategyContext
    {
        private IDepartmentStrategy _departmentStrategy { get; set; }

        public void SetStrategy(IDepartmentStrategy departmentStrategy)
        {
            _departmentStrategy = departmentStrategy;
        }

        public async Task<Department> ExecuteStrategy()
        {
            return await _departmentStrategy.Execute();
        }
    }
}
