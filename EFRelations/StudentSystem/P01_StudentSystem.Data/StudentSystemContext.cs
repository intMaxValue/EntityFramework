using Microsoft.EntityFrameworkCore;
using StudentSystem.P01_StudentSystem.Data.Models;
using StudentSystem.P01_StudentSystem.Data.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentSystem.P01_StudentSystem.Data
{
    public class StudentSystemContext : DbContext
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<Resource> Resources { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ResourceConfiguration());


        }
    }
}
