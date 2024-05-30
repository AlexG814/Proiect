using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PoliceApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace PoliceApp.Controllers
{
    [Authorize]
    public class CazuriController : Controller
    {
        private readonly PoliceDbContext _context;
        private readonly ILogger<CazuriController> _logger;

        public CazuriController(PoliceDbContext context, ILogger<CazuriController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Cazuri
        public async Task<IActionResult> Index(string searchString)
        {
            var cazuri = from c in _context.Cazuri.Include(c => c.Comentarii)
                         where !c.Arhivat
                         select c;

            if (!string.IsNullOrEmpty(searchString))
            {
                cazuri = cazuri.Where(c => c.Descriere.Contains(searchString));
            }

            return View(await cazuri.ToListAsync());
        }

        // GET: Cazuri/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cazuri/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Descriere,Stadiu,Prioritate,UtilizatorId")] Caz caz)
        {
            if (ModelState.IsValid)
            {
                _context.Add(caz);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(caz);
        }

        // GET: Cazuri/Edit/2
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var caz = await _context.Cazuri.FindAsync(id);
            if (caz == null)
            {
                return NotFound();
            }
            return View(caz);
        }

        // POST: Cazuri/Edit/2
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CazId,Descriere,Stadiu,Prioritate,UtilizatorId")] Caz caz)
        {
            if (id != caz.CazId)
            {
                _logger.LogError("Caz ID mismatch.");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(caz);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "Concurrency error occurred while updating the case.");
                    if (!CazExists(caz.CazId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(caz);
        }

        // GET: Cazuri/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var caz = await _context.Cazuri
                .Include(c => c.Comentarii)
                .FirstOrDefaultAsync(m => m.CazId == id);
            if (caz == null)
            {
                return NotFound();
            }

            return View(caz);
        }

        // GET: Cazuri/AdaugaComentariu/5
        public IActionResult AdaugaComentariu(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.CazId = id;
            return View();
        }

        // POST: Cazuri/AdaugaComentariu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdaugaComentariu([Bind("Continut,CazId")] Comentariu comentariu)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Comentarii.Add(comentariu);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Comentariu adăugat cu succes.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while adding the comment.");
                    return View(comentariu); // Afișează formularul din nou dacă există o eroare
                }
                return RedirectToAction(nameof(Details), new { id = comentariu.CazId }); // Redirecționează după succes
            }
            else
            {
                _logger.LogWarning("Model state is invalid. Errors: {@Errors}", ModelState.Values.SelectMany(v => v.Errors));
            }
            ViewBag.CazId = comentariu.CazId;
            return View(comentariu); // Afișează formularul din nou dacă modelul nu este valid
        }

        // DELETE: Cazuri/DeleteComentariu/5
        [HttpPost, ActionName("DeleteComentariu")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComentariu(int id)
        {
            var comentariu = await _context.Comentarii.FindAsync(id);
            if (comentariu != null)
            {
                _context.Comentarii.Remove(comentariu);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Comentariu șters cu succes.");
            }
            return RedirectToAction(nameof(Details), new { id = comentariu.CazId });
        }

        // GET: Cazuri/Arhiva
        public async Task<IActionResult> Arhiva(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var arhiva = from c in _context.Cazuri
                         where c.Arhivat
                         select c;

            if (!string.IsNullOrEmpty(searchString))
            {
                arhiva = arhiva.Where(c => c.Descriere.Contains(searchString));
            }

            return View(await arhiva.ToListAsync());
        }

        // POST: Cazuri/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var caz = await _context.Cazuri.FindAsync(id);
            caz.Arhivat = true; // Marchează cazul ca arhivat în loc să fie șters
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Cazuri/DeletePermanent/5
        [HttpPost, ActionName("DeletePermanent")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePermanent(int id)
        {
            var caz = await _context.Cazuri.FindAsync(id);
            if (caz != null)
            {
                _context.Cazuri.Remove(caz);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Caz șters definitiv cu succes.");
            }
            return RedirectToAction(nameof(Arhiva));
        }

        private bool CazExists(int id)
        {
            return _context.Cazuri.Any(e => e.CazId == id);
        }
    }
}
