using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PoliceApp.Models;

namespace PoliceApp.Controllers
{
    public class UtilizatorsController : Controller
    {
        private readonly PoliceDbContext _context;
        private readonly ILogger<UtilizatorsController> _logger;

        public UtilizatorsController(PoliceDbContext context, ILogger<UtilizatorsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Utilizators
        public async Task<IActionResult> Index()
        {
            return View(await _context.Utilizatori.ToListAsync());
        }

        // GET: Utilizators/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizator = await _context.Utilizatori
                .FirstOrDefaultAsync(m => m.UtilizatorId == id);
            if (utilizator == null)
            {
                return NotFound();
            }

            return View(utilizator);
        }

        // GET: Utilizators/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Utilizators/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UtilizatorId,Nume,Parola,TipUtilizator")] Utilizator utilizator)
        {
            if (ModelState.IsValid)
            {
                _context.Add(utilizator);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(utilizator);
        }

        // GET: Utilizators/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizator = await _context.Utilizatori.FindAsync(id);
            if (utilizator == null)
            {
                return NotFound();
            }
            return View(utilizator);
        }

        // POST: Utilizators/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UtilizatorId,Nume,Parola,TipUtilizator")] Utilizator utilizator)
        {
            if (id != utilizator.UtilizatorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(utilizator);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UtilizatorExists(utilizator.UtilizatorId))
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
            return View(utilizator);
        }

        // GET: Utilizators/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizator = await _context.Utilizatori
                .FirstOrDefaultAsync(m => m.UtilizatorId == id);
            if (utilizator == null)
            {
                return NotFound();
            }

            return View(utilizator);
        }

        // POST: Utilizators/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var utilizator = await _context.Utilizatori.FindAsync(id);
            if (utilizator != null)
            {
                _context.Utilizatori.Remove(utilizator);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UtilizatorExists(int id)
        {
            return _context.Utilizatori.Any(e => e.UtilizatorId == id);
        }
    }
}
