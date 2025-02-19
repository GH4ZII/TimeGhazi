using Microsoft.AspNetCore.Authorization; // Gir tilgang til autorisering (roller og tilgangskontroll)
using Microsoft.AspNetCore.Mvc; // Gir tilgang til MVC-funksjonalitet (kontrollere, visninger)
using Microsoft.AspNetCore.SignalR; // Brukes til sanntidskommunikasjon med SignalR
using Microsoft.EntityFrameworkCore; // Gir tilgang til databasen via Entity Framework Core
using TimeGhazi.Models; // Importerer prosjektets modeller
using TimeGhazi.Hubs; // Importerer SignalR-hubben for sanntidsoppdateringer

namespace TimeGhazi.Controllers
{
    // **Kun admin har tilgang til denne kontrolleren**
    [Authorize(Roles = "Admin")]
    public class ShiftController : Controller
    {
        // **Database og SignalR-hub**
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<ShiftHub> _hubContext;

        // **Konstruktør som setter opp avhengighetene**
        public ShiftController(ApplicationDbContext context, IHubContext<ShiftHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // **Viser admin-panelet med alle skift**
        public async Task<IActionResult> Index()
        {
            var shifts = await _context.Shifts.ToListAsync(); // Henter alle skift fra databasen
            return View(shifts); // Sender skift-listen til visningen
        }

        // **GET: Viser skjema for å legge til nytt skift**
        [Authorize(Roles = "Admin")] // Kun admin kan se skjemaet
        public IActionResult Create()
        {
            return View(); // Viser skjema for å opprette skift
        }

        // **POST: Legger til et nytt skift**
        [HttpPost]
        [Authorize(Roles = "Admin")] // Kun admin kan legge til skift
        [ValidateAntiForgeryToken] // Beskytter mot CSRF-angrep
        public async Task<IActionResult> Create([Bind("EmployeeId,StartTime,EndTime")] Shift shift)
        {
            // **Sjekker om skjemaet er fylt ut riktig**
            if (ModelState.IsValid)
            {
                shift.IsApproved = false; // **Nytt skift starter som ikke godkjent**
                _context.Add(shift); // Legger skiftet i databasen
                await _context.SaveChangesAsync(); // Lagrer endringer i databasen

                // **Sender sanntidsoppdatering til alle tilkoblede klienter via SignalR**
                await _hubContext.Clients.All.SendAsync("ReceiveShiftUpdate", "Et nytt skift har blitt lagt til!");

                return RedirectToAction(nameof(Index)); // Går tilbake til listen over skift
            }

            return View(shift); // Hvis feil, returnerer skjemaet med feilmeldinger
        }
    }
}
