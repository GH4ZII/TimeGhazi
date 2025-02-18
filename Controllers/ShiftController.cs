using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TimeGhazi.Models;
using TimeGhazi.Hubs;

namespace TimeGhazi.Controllers
{
    public class ShiftController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<ShiftHub> _hubContext;

        public ShiftController(ApplicationDbContext context, IHubContext<ShiftHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // Vis admin-panelet med alle skift
        public async Task<IActionResult> Index()
        {
            var shifts = await _context.Shifts.ToListAsync();
            return View(shifts);
        }

        // GET: Vise skjema for å legge til nytt skift
        public IActionResult Create()
        {
            return View();
        }

        // POST: Legge til nytt skift
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,StartTime,EndTime")] Shift shift)
        {
            if (ModelState.IsValid)
            {
                shift.IsApproved = false; // Nytt skift starter som ikke godkjent
                _context.Add(shift);
                await _context.SaveChangesAsync();

                // Send sanntidsoppdatering til mobilappen
                await _hubContext.Clients.All.SendAsync("ReceiveShiftUpdate", "Et nytt skift har blitt lagt til!");

                return RedirectToAction(nameof(Index));
            }
            return View(shift);
        }
    }
}