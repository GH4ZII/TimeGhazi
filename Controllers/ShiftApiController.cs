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
        // Gir oss tilgang til databasen
        private readonly ApplicationDbContext _context;
        
        // Gir oss tilgang til SignalR-hubben for sanntidskommunikasjon
        private readonly IHubContext<ShiftHub> _hubContext;

        // Konstruktør som setter opp avhengigheter (database og SignalR)
        public ShiftApiController(ApplicationDbContext context, IHubContext<ShiftHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // GET: api/shifts
        [HttpGet] 
        // Henter og returnerer alle skift fra databasen
        public async Task<ActionResult<IEnumerable<Shift>>> GetShifts()
        {
            return await _context.Shifts.ToListAsync(); // Henter alle skift i en liste
        }
        
        // POST: api/shifts
        [HttpPost]
        // Oppretter et nytt skift i databasen
        public async Task<ActionResult<Shift>> CreateShift([FromBody] Shift shift)
        {
            // Sjekker om det er sendt inn gyldig skiftdata
            if (shift == null)
                return BadRequest("Ugyldig skiftdata");

            // Legger til skiftet i databasen
            _context.Shifts.Add(shift);
            await _context.SaveChangesAsync();

            // Sender sanntidsoppdatering til alle tilkoblede klienter via SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveShiftUpdate", "Et nytt skift har blitt lagt til!");

            // Returnerer en bekreftelse på at skiftet ble opprettet
            return CreatedAtAction(nameof(GetShifts), new { id = shift.Id }, shift);
        }
    } 
}
