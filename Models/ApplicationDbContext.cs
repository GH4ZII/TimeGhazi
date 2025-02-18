using Microsoft.EntityFrameworkCore;

namespace TimeGhazi.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Shift> Shifts { get; set; }
    }
}