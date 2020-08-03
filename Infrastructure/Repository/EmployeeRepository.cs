using Core.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Repository
{
    public class EmployeeRepository : GenericRepository<Employee>
    {
        public EmployeeRepository(DbContext context) : base(context)
        {
        }

        public Employee CheckLogin(string emailAddress, string password)
        {
            Login loginMember = _context.Set<Login>().Where(item => item.EmailAddress.Equals(emailAddress) && item.Password.Equals(password)).FirstOrDefault();
            Employee employee = _context.Set<Employee>().Where(item => item.Id == loginMember.EmployeeId).FirstOrDefault();
            return employee;
        }

        public Employee SearchForEmployeeByEmail(string emailAddress)
        {
           Employee employee = _context.Set<Employee>().Where(item => item.EmailAddress.Equals(emailAddress)).FirstOrDefault();
           return employee;
        }

        public List<Employee> RetrieveManagersEmployees(int managerId)
        {
            List<Employee> result;
            result = _context.Set<Employee>().Where(item => item.ManagerId == managerId).ToList();
            return result;
        }

    }
}
