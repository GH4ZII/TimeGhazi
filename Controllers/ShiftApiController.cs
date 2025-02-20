using Microsoft.AspNetCore.Mvc; // Gir tilgang til MVC-funksjonalitet (API-kontrollere)
using Microsoft.AspNetCore.SignalR; // Brukes til sanntidskommunikasjon med SignalR
using Microsoft.EntityFrameworkCore; // Gir tilgang til databasen via Entity Framework Core
using TimeGhazi.Models; // Importerer prosjektets modeller
using TimeGhazi.Hubs; // Importerer SignalR-hubben for sanntidsoppdateringer

namespace TimeGhazi.Controllers
{
    // Angir at denne API-kontrolleren håndterer forespørsler til "api/shifts"
    [Route("api/shifts")]
    
    // Forteller at dette er en API-kontroller (ingen visninger, kun data)
    [ApiController]
    public class ShiftApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<ShiftHub> _hubContext;

        public ShiftApiController(ApplicationDbContext context, IHubContext<ShiftHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // 📌 GET: api/shifts/{employeeId}
        // 🔹 Henter skift KUN for en spesifikk ansatt
        [HttpGet("{employeeId}")]
        public async Task<ActionResult<IEnumerable<Shift>>> GetShiftsForEmployee(int employeeId)
        {
            Console.WriteLine($"📡 Henter skift for ansatt {employeeId}");

            var shifts = await _context.Shifts
                .Where(s => s.EmployeeId == employeeId)
                .ToListAsync();

            if (shifts == null || shifts.Count == 0)
            {
                Console.WriteLine($"❌ Ingen skift funnet for ansatt {employeeId}");
                return NotFound(new { message = "Ingen skift funnet." });
            }

            return Ok(shifts);
        }

        // 📌 POST: api/shifts
        // 🔹 Oppretter et nytt skift
        [HttpPost]
        public async Task<ActionResult<Shift>> CreateShift([FromBody] Shift shift)
        {
            if (shift == null)
            {
                Console.WriteLine("❌ Ugyldig skiftdata mottatt!");
                return BadRequest("Ugyldig skiftdata");
            }

            // Sjekker om den ansatte finnes i databasen
            var employeeExists = await _context.Employees.AnyAsync(e => e.Id == shift.EmployeeId);
            if (!employeeExists)
            {
                Console.WriteLine($"❌ Ansatt med ID {shift.EmployeeId} ikke funnet!");
                return BadRequest("Ansatt ikke funnet");
            }

            Console.WriteLine($"✅ Oppretter nytt skift for ansatt {shift.EmployeeId}");
            shift.IsApproved = true; // Skiftet blir automatisk godkjent

            _context.Shifts.Add(shift);
            await _context.SaveChangesAsync();

            // 📡 Send sanntidsoppdatering KUN til riktig ansatt via SignalR
            await _hubContext.Clients.Group(shift.EmployeeId.ToString())
                .SendAsync("ReceiveShiftUpdate", $"Et nytt skift er lagt til for deg!");

            return CreatedAtAction(nameof(GetShiftsForEmployee), new { employeeId = shift.EmployeeId }, shift);
        }
    } 
}
