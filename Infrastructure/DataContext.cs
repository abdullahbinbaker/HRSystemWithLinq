using Core;
using Core.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {

        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<Login> Logins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity <Login> ().Property(m => m.EmployeeId).ValueGeneratedNever();
            modelBuilder.Entity<Employee>().HasData(
                new Employee(1,"abdullah salem", "Admin@gmail.com", "0568039837", "Male", "Admin", new DateTime(200, 04, 06),
                   1, 30)
                ); ;

            modelBuilder.Entity<Login>().HasData(
              new Login(1,"Admin@gmail.com", "Admin")
              ); 
        }

       
    }
}
