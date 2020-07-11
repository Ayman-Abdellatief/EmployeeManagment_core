using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagment.Models
{
    public static class ModuleBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasData(
               new Employee
               {
                   Id = 1
                   ,
                   Name = "Ayman"
                   ,
                   Department = Dept.It
                   ,
                   Email = "Ayman@ayman.com"
               },
                 new Employee
                 {
                     Id = 2
                   ,
                     Name = "Arwa"
                   ,
                     Department = Dept.Hr
                   ,
                     Email = "Arwa@ar.com"
                 }

               );
        }
    }
}
