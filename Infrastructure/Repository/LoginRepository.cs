using Core.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Repository
{
    public class LoginRepository : GenericRepository<Login>
    {
        public LoginRepository(DbContext context) : base(context)
        {
        }

        public string RetrievePasswordByEmail(string email)
        {
           Login login = _context.Set<Login>().Where(item => item.EmailAddress.Equals(email)).FirstOrDefault();
           string password = login.Password.ToString();
           return password;
        }
    }
}
