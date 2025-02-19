using Microsoft.AspNetCore.Identity; // Importerer Identity for brukerhåndtering

namespace TimeGhazi.Models
{
    // **DataSeeder er ansvarlig for å opprette standardroller og en admin-bruker ved oppstart**
    public class DataSeeder
    {
        // **Metode for å opprette admin-brukeren og standardroller**
        public static async Task SeedAdminUser(IServiceProvider serviceProvider)
        {
            // **Henter UserManager og RoleManager fra services**
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // **Definerer standard admin-bruker**
            string adminEmail = "admin@example.com";
            string adminPassword = "Admin123!";

            // **Sjekker om "Admin"-rollen finnes, hvis ikke, opprett den**
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // **Sjekker om "Employee"-rollen finnes, hvis ikke, opprett den**
            if (!await roleManager.RoleExistsAsync("Employee"))
            {
                await roleManager.CreateAsync(new IdentityRole("Employee"));
            }

            // **Sjekker om admin-brukeren finnes i systemet**
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                // **Oppretter en ny admin-bruker**
                var user = new IdentityUser 
                { 
                    UserName = adminEmail, 
                    Email = adminEmail, 
                    EmailConfirmed = true // Admin-brukeren bekreftes automatisk
                };

                // **Oppretter admin-brukeren med et standardpassord**
                await userManager.CreateAsync(user, adminPassword);

                // **Legger admin-brukeren til "Admin"-rollen**
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}
