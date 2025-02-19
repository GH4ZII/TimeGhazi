using Microsoft.AspNetCore.Identity; // Importerer Identity for brukerhåndtering
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // Brukes for IdentityDbContext
using Microsoft.EntityFrameworkCore; // Gir tilgang til Entity Framework Core

namespace TimeGhazi.Models
{
    // **Definerer databasekonteksten, som inkluderer Identity for brukere og roller**
    public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        // **Konstruktør som konfigurerer databasealternativer**
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

        // **Definerer en tabell for ansatte (Employees)**
        public DbSet<Employee> Employees { get; set; }

        // **Definerer en tabell for skift (Shifts)**
        public DbSet<Shift> Shifts { get; set; }

        // **Tilpasser Identity-tabellene om nødvendig**
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Beholder standard Identity-tabeller
        }
    }
}