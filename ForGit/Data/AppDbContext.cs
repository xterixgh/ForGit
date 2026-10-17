using ForGit.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForGit.Data
{
    public class AppDbContext : DbContext
       
    {
        public AppDbContext()
        {

        }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=ForGit;Username=postgres;Password=Bill0405h/;");
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Logins> Logins { get; set; }
        public DbSet<Roles> Roles { get; set; }

    }
}
