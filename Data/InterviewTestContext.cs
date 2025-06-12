using Microsoft.EntityFrameworkCore;
using EmployeeDataProject.Models;

namespace EmployeeDataProject.Data
{
    public class InterviewTestContext : DbContext
    {
        public InterviewTestContext(DbContextOptions<InterviewTestContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<CodeRef> CodeRefs { get; set; }
        public DbSet<Company> Companies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .HasKey(e => new { e.CompanyCode, e.EmpCode });

            modelBuilder.Entity<CodeRef>()
                .HasKey(c => new { c.CodeType, c.Code });

        }
    }
}
