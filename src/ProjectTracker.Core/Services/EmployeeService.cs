using ProjectTracker.Core.Interfaces;
using ProjectTracker.Models.Constants;
using ProjectTracker.Models.Models.Entities;

namespace ProjectTracker.Core.Services
{
    public class EmployeeService
    {
        private readonly IEmployeeRepository _repository;

        public EmployeeService(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        public List<Employee> GetAll() => _repository.GetAll();

        public Employee? GetById(int id) => _repository.GetById(id);

        public Employee? GetByUserId(string userId) => _repository.GetByUserId(userId);
    }
}
