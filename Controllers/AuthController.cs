using Microsoft.AspNetCore.Identity; // Brukes til å håndtere brukere i systemet
using Microsoft.AspNetCore.Mvc; // Gjør det mulig å lage en API-kontroller
using Microsoft.IdentityModel.Tokens; // Brukes til å lage en sikker login-token (JWT)
using System.IdentityModel.Tokens.Jwt; // Brukes til å generere og lese tokens
using System.Security.Claims; // Brukes til å lagre informasjon i tokenet
using System.Text; // Brukes til å håndtere tekstdata
using TimeGhazi.Models; // Importerer vår database-modell

namespace TimeGhazi.Controllers
{
    // **Denne klassen lager et API som håndterer login**
    [Route("api/auth")] // Alle URL-er starter med /api/auth
    [ApiController] // Forteller at dette er en API-kontroller
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager; // Håndterer brukere
        private readonly SignInManager<IdentityUser> _signInManager; // Håndterer innlogging
        private readonly ApplicationDbContext _context; // Kobler til databasen
        private readonly IConfiguration _config; // Brukes til å hente hemmelige nøkler fra `appsettings.json`

        // **Konstruktør: Koden som kjører når vi lager denne klassen**
        public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, 
                              ApplicationDbContext context, IConfiguration config)
        {
            _userManager = userManager; // Setter opp brukerhåndtering
            _signInManager = signInManager; // Setter opp innlogging
            _context = context; // Setter opp databasen
            _config = config; // Leser inn konfigurasjon
        }

        // **Dette er API-endepunktet for login**
        [HttpPost("login")] // Brukeren sender POST-forespørsel til /api/auth/login
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            // **Sjekker om brukeren finnes i databasen**
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) // Hvis brukeren ikke finnes
            {
                return Unauthorized(new { message = "Feil e-post eller passord" });
            }

            // **Sjekker om passordet er riktig**
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
            if (!result.Succeeded) // Hvis passordet er feil
            {
                return Unauthorized(new { message = "Feil e-post eller passord" });
            }

            // **Sjekker om brukeren finnes i Employee-tabellen**
            var employee = _context.Employees.FirstOrDefault(e => e.UserId == user.Id);
            if (employee == null) // Hvis brukeren ikke er en ansatt
            {
                return Unauthorized(new { message = "Kun ansatte kan logge inn" });
            }

            // **Genererer en token (nøkkel) som brukes for sikkerhet**
            var token = GenerateJwtToken(user);
            return Ok(new { token, employeeId = employee.Id}); // Returnerer tokenet til brukeren
        }

        // **Denne funksjonen lager en sikker token (nøkkel)**
        private string GenerateJwtToken(IdentityUser user)
        {
            // Henter hemmelig nøkkel fra appsettings.json
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Lager informasjon som legges i tokenet
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id), // Brukerens ID
                new Claim(JwtRegisteredClaimNames.Email, user.Email), // Brukerens e-post
                new Claim(ClaimTypes.Role, "Employee") // Brukeren har rollen "Employee"
            };

            // Lager selve tokenet med informasjonen
            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"], // Hvem som lagde tokenet
                _config["Jwt:Issuer"], // Hvem som kan bruke tokenet
                claims,
                expires: DateTime.UtcNow.AddHours(12), // Tokenet varer i 12 timer
                signingCredentials: creds // Signerer tokenet for sikkerhet
            );

            return new JwtSecurityTokenHandler().WriteToken(token); // Returnerer tokenet som en tekststreng
        }
    }

    // **Modell for login-data (e-post og passord)**
    public class LoginRequest
    {
        public string Email { get; set; } // Brukerens e-post
        public string Password { get; set; } // Brukerens passord
    }
}
