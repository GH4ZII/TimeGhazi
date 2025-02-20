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
            var shifts = await _context.Shifts
                .Include(s => s.Employee) // 🔥 Henter ansatt-informasjonen
                .ToListAsync();

            return View(shifts); // Sender skift-listen til visningen
        }

        // **GET: Viser skjema for å legge til nytt skift**
        [Authorize(Roles = "Admin")] // Kun admin kan se skjemaet
        public IActionResult Create()
        {
            
            var employees = _context.Employees.ToList();

            if (employees == null || !employees.Any())
            {
                ViewBag.Employees = new List<Employee>();
            }
            else
            {
                ViewBag.Employees = employees;
            }
            return View(); // Viser skjema for å opprette skift
        }

        // **POST: Legger til et nytt skift**
        [HttpPost]
        [Authorize(Roles = "Admin")] // Kun admin kan legge til skift
        [ValidateAntiForgeryToken] // Beskytter mot CSRF-angrep
public async Task<IActionResult> Create([Bind("EmployeeId,StartTime,EndTime")] Shift shift)
{
    var employees = _context.Employees.ToList();
    ViewBag.Employees = employees; // Sikrer at dropdown for ansatte fungerer

    Console.WriteLine("🟢 Mottatt forespørsel til Create-metoden.");
    Console.WriteLine($"🔹 EmployeeId: {shift.EmployeeId}");
    Console.WriteLine($"🔹 StartTime: {shift.StartTime}");
    Console.WriteLine($"🔹 EndTime: {shift.EndTime}");

    if (!ModelState.IsValid)
    {
        Console.WriteLine("❌ ModelState er ikke valid! Feilmeldinger:");
        foreach (var error in ModelState)
        {
            foreach (var err in error.Value.Errors)
            {
                Console.WriteLine($"  - {error.Key}: {err.ErrorMessage}");
            }
        }
        return View(shift);
    }

    try
    {
        // 🔥 Hent Employee fra databasen basert på EmployeeId
        var employee = await _context.Employees.FindAsync(shift.EmployeeId);
        if (employee == null)
        {
            Console.WriteLine("❌ Feil: Fant ikke en ansatt med ID " + shift.EmployeeId);
            ModelState.AddModelError("EmployeeId", "Den valgte ansatte finnes ikke.");
            return View(shift);
        }

        shift.Employee = employee; // 🔥 Viktig! Setter Employee-objektet

        shift.IsApproved = true; // **Nytt skift starter som ikke godkjent**
        _context.Add(shift);
        await _context.SaveChangesAsync();
        Console.WriteLine("✅ Skift lagret i databasen!");

        // **Sender sanntidsoppdatering til alle tilkoblede klienter via SignalR**
        await _hubContext.Clients.All.SendAsync("ReceiveShiftUpdate", "Et nytt skift har blitt lagt til!");
        Console.WriteLine("📡 SignalR-melding sendt: Et nytt skift har blitt lagt til!");

        return RedirectToAction(nameof(Index)); // Går tilbake til listen over skift
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Feil ved lagring i databasen: {ex.Message}");
        return View(shift);
            }
        }
    }
}

