using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PSKCrackers.Data;
using PSKCrackers.Helpers;
using PSKCrackers.Models;

namespace PSKCrackers.Controllers
{
    public class InventoryLocationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InventoryLocationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: InventoryLocations
        public async Task<IActionResult> Index()
        {
              return _context.InventoryLocations != null ? 
                          View(await _context.InventoryLocations.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.InventoryLocations'  is null.");
        }

        // GET: InventoryLocations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.InventoryLocations == null)
            {
                return NotFound();
            }

            var inventoryLocation = await _context.InventoryLocations
                .FirstOrDefaultAsync(m => m.InventoryLocationId == id);
            if (inventoryLocation == null)
            {
                return NotFound();
            }

            return View(inventoryLocation);
        }

        // GET: InventoryLocations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: InventoryLocations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InventoryLocationId,Name,Description")] InventoryLocation inventoryLocation)
        {
            Utils.removeVirtualProperties(inventoryLocation, ModelState);
            if (ModelState.IsValid)
            {
                _context.Add(inventoryLocation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(inventoryLocation);
        }

        // GET: InventoryLocations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.InventoryLocations == null)
            {
                return NotFound();
            }

            var inventoryLocation = await _context.InventoryLocations.FindAsync(id);
            if (inventoryLocation == null)
            {
                return NotFound();
            }
            return View(inventoryLocation);
        }

        // POST: InventoryLocations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InventoryLocationId,Name,Description")] InventoryLocation inventoryLocation)
        {
            if (id != inventoryLocation.InventoryLocationId)
            {
                return NotFound();
            }
            Utils.removeVirtualProperties(inventoryLocation, ModelState);
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inventoryLocation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InventoryLocationExists(inventoryLocation.InventoryLocationId))
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
            return View(inventoryLocation);
        }

        // GET: InventoryLocations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.InventoryLocations == null)
            {
                return NotFound();
            }

            var inventoryLocation = await _context.InventoryLocations
                .FirstOrDefaultAsync(m => m.InventoryLocationId == id);
            if (inventoryLocation == null)
            {
                return NotFound();
            }

            return View(inventoryLocation);
        }

        // POST: InventoryLocations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.InventoryLocations == null)
            {
                return Problem("Entity set 'ApplicationDbContext.InventoryLocations'  is null.");
            }
            var inventoryLocation = await _context.InventoryLocations.FindAsync(id);
            if (inventoryLocation != null)
            {
                _context.InventoryLocations.Remove(inventoryLocation);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InventoryLocationExists(int id)
        {
          return (_context.InventoryLocations?.Any(e => e.InventoryLocationId == id)).GetValueOrDefault();
        }
    }
}
