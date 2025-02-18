using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TimeGhazi.Models;
using TimeGhazi.Hubs;

namespace TimeGhazi.Controllers
{
    // Angir at URL'en starter med api/shifts
    [Route("api/shifts")]
    
    // forteller at dette er en api-kontroller.
    [ApiController]
    public class ShiftApiController : ControllerBase
    {
        // Gir oss tilgang til databasen
        private readonly ApplicationDbContext _context;
        
        // Gir oss tilgang til SignalR-hubben
        private readonly IHubContext<ShiftHub> _hubContext;

        public ShiftApiController(ApplicationDbContext context, IHubContext<ShiftHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // GET: api/shifts
        [HttpGet] 
        // Returnerer en liste over alle skiftene
        public async Task<ActionResult<IEnumerable<Shift>>> GetShifts()
        {
            return await _context.Shifts.ToListAsync();
        }
        

        // POST: api/shifts
        [HttpPost]
        // Legger til et nytt skift
        public async Task<ActionResult<Shift>> CreateShift([FromBody] Shift shift)
        {
            if (shift == null)
                return BadRequest("Ugyldig skiftdata");

            _context.Shifts.Add(shift);
            await _context.SaveChangesAsync();

            // Send sanntidsoppdatering til mobilappen via SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveShiftUpdate", "Et nytt skift har blitt lagt til!");

            return CreatedAtAction(nameof(GetShifts), new { id = shift.Id }, shift);
        }
    } 
}