using Microsoft.EntityFrameworkCore;
using ProjectTracker.Core.Interfaces;
using ProjectTracker.Data;
using ProjectTracker.Models.Models.Entities;

namespace ProjectTracker.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Employee> GetAll() =>
            _context.Employees.OrderBy(e => e.LastName).ThenBy(e => e.FirstName).ToList();

        public Employee? GetById(int id) =>
            _context.Employees.FirstOrDefault(x => x.Id == id);

        public Employee? GetByUserId(string userId) =>
            _context.Employees.FirstOrDefault(x => x.UserId == userId);

        public void Add(Employee employee)
        {
            _context.Employees.Add(employee);
            _context.SaveChanges();
        }

        public void Update(Employee employee)
        {
            _context.Employees.Update(employee);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var employee = GetById(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                _context.SaveChanges();
            }
        }

        public bool Exists(int id) => _context.Employees.Any(x => x.Id == id);

        public int Count() => _context.Employees.Count();
    }
}
