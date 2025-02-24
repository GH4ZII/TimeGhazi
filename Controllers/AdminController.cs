using Microsoft.AspNetCore.Authorization; // Gir tilgang til autorisering (roller, tilgangskontroll)
using Microsoft.AspNetCore.Identity; // Gir tilgang til brukerhåndtering i ASP.NET Identity
using Microsoft.AspNetCore.Mvc; // Gir tilgang til MVC-funksjonalitet (kontrollere, views)
using TimeGhazi.Models; // Importerer prosjektets modeller
using System.Threading.Tasks; // Gir støtte for asynkrone operasjoner
using Microsoft.EntityFrameworkCore; // Gir tilgang til databasen via Entity Framework Core

namespace TimeGhazi.Controllers
{
    // Krever at brukeren er admin for å få tilgang til denne kontrolleren
    [Authorize(Roles = "Admin")] 
    public class AdminController : Controller
    {
        // Definerer avhengigheter for brukerhåndtering, roller og database
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        // Konstruktør som setter opp avhengighetene
        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // GET: Viser skjemaet for å opprette en ny ansatt
        public IActionResult CreateEmployee()
        {
            return View();
        }

        // POST: Oppretter en ny ansatt i systemet
        [HttpPost]
        [ValidateAntiForgeryToken] // Hindrer CSRF-angrep
        public async Task<IActionResult> CreateEmployee(Employee employee)
        {
            // Fjerner validering for UserId og User, siden de blir satt manuelt senere
            ModelState.Remove("UserId");
            ModelState.Remove("User");

            // Logger mottatt data fra skjemaet
            Console.WriteLine($"🔍 Mottatt data fra skjema: Name={employee.Name}, Email={employee.Email}, Phone={employee.PhoneNumber}, Role={employee.Role}, Age={employee.Age}, Address={employee.Address}");

            // Sjekker om skjemaet er gyldig
            if (!ModelState.IsValid)
            {
                Console.WriteLine("❌ ModelState er ikke gyldig!");
                foreach (var key in ModelState.Keys)
                {
                    foreach (var error in ModelState[key].Errors)
                    {
                        Console.WriteLine($"❌ Feil i {key}: {error.ErrorMessage}");
                    }
                }
                return View(employee); // Returnerer skjemaet med feilmelding
            }

            // Sjekker om brukeren allerede finnes i systemet
            var existingUser = await _userManager.FindByEmailAsync(employee.Email);
            if (existingUser != null)
            {
                Console.WriteLine("❌ Bruker eksisterer allerede.");
                ModelState.AddModelError("", "En bruker med denne e-posten eksisterer allerede.");
                return View(employee);
            }

            // Oppretter en ny bruker i Identity-systemet med et midlertidig passord
            string tempPassword = "TempPass123!";
            var user = new IdentityUser
            {
                UserName = employee.Email,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                EmailConfirmed = true // Bekrefter e-post automatisk
            };

            var result = await _userManager.CreateAsync(user, tempPassword);
            if (result.Succeeded) // Sjekker om brukeren ble opprettet
            {
                Console.WriteLine("✅ Bruker opprettet i AspNetUsers!");

                // Sjekker om rollen "Employee" finnes, og oppretter den hvis ikke
                if (!await _roleManager.RoleExistsAsync("Employee"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Employee"));
                }

                // Legger brukeren til rollen "Employee"
                await _userManager.AddToRoleAsync(user, "Employee");

                // Oppretter en ny ansatt og kobler den til den nye brukeren
                var newEmployee = new Employee
                {
                    Name = employee.Name,
                    Email = employee.Email,
                    PhoneNumber = employee.PhoneNumber,
                    Role = employee.Role, // Setter rollen basert på input
                    Age = employee.Age,
                    Address = employee.Address,
                    UserId = user.Id // Knytter ansatte til brukeren i Identity
                };

                // Lagrer den nye ansatte i databasen
                _context.Employees.Add(newEmployee);
                await _context.SaveChangesAsync();
                Console.WriteLine($"✅ Ansatt {newEmployee.Name} lagret i databasen med ID: {newEmployee.Id}");

                // Viser en bekreftelse til admin
                ViewBag.Message = "Brukeren er opprettet. Gi dem dette passordet: " + tempPassword;
                return View();
            }

            // Hvis brukeropprettelsen feiler, vises feilmeldingene
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
                Console.WriteLine($"❌ Feil ved oppretting av bruker: {error.Description}");
            }

            return View(employee);
        }
    }
}
