using ProjectTracker.Models.Models.Entities;

namespace ProjectTracker.Core.Interfaces
{
    public interface IEmployeeRepository
    {
        List<Employee> GetAll();
        Employee? GetById(int id);
        Employee? GetByUserId(string userId);
        void Add(Employee employee);
        void Update(Employee employee);
        void Delete(int id);
        bool Exists(int id);
        int Count();
    }
}
