using Core.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Repository
{
    public class HolidayRepository : GenericRepository<Holiday>
    {
        public HolidayRepository(DbContext context) : base(context)
        {
        }

        public void UpdateHolidayStatus(int id, String holidayStatus)
        {
            Holiday holiday= _context.Set<Holiday>().Where(item => item.Id == id).FirstOrDefault();
            holiday.HolidayStatus = holidayStatus;
          }

        public List<Holiday> GetHolidaysByEmployeeId(int id)
        {
             List<Holiday> results;
             results = _context.Set<Holiday>().Where(item => item.EmployeeId == id).ToList();
             return results;
        }

        public List<Holiday> ListVacationsByEmployeeIdAndStatus(int id, string status)
        {
            List<Holiday> results;
            results = _context.Set<Holiday>().Where(item => item.EmployeeId == id && item.HolidayStatus.Equals(status)).ToList();
            return results;
        }

        public List<Holiday> ListVacationsByEmployeeIdAndStartDate(int id, DateTime startDate)
        {
            List<Holiday> results;
            results = _context.Set<Holiday>().Where(item => item.EmployeeId == id && item.HolidayStartDate == startDate).ToList();
            return results;
        }

        public List<Holiday> DisplayEmployeesHolidayRequests(string status)
        {
            List<Holiday> results;
            results = _context.Set<Holiday>().Where(item => item.HolidayStatus.Equals(status)).ToList();
            return results;
        }
    }
}
