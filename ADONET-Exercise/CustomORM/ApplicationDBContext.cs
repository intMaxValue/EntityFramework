using CustomORM.Minions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomORM
{
    public class ApplicationDBContext : DbContext
    {
        private const string connectionString =
            @"Server=SKYNET\SQLEXPRESS;Database=ADONETMinionsDB;Integrated Security=True;TrustServerCertificate=True";

        public DbSet<Towns> Towns { get; set; }

        public DbSet<Countries> Countries { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }

        
    }
}
