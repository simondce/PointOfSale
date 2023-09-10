using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PSKCrackers.Data;
using PSKCrackers.Models;

namespace PSKCrackers.Controllers
{
    public class GoodsReceivedsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GoodsReceivedsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: GoodsReceiveds
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.GoodsReceived.Include(g => g.Supplier);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: GoodsReceiveds/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.GoodsReceived == null)
            {
                return NotFound();
            }

            var goodsReceived = await _context.GoodsReceived
                .Include(g => g.Supplier)
                .FirstOrDefaultAsync(m => m.GoodsReceivedId == id);
            if (goodsReceived == null)
            {
                return NotFound();
            }

            return View(goodsReceived);
        }

        // GET: GoodsReceiveds/Create
        public IActionResult Create()
        {
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "Name");
            return View();
        }

        // POST: GoodsReceiveds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GoodsReceivedId,ReceiptDate,TotalReceivedCost,SupplierId")] GoodsReceived goodsReceived)
        {
            if (ModelState.IsValid)
            {
                _context.Add(goodsReceived);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "Name", goodsReceived.SupplierId);
            return View(goodsReceived);
        }

        // GET: GoodsReceiveds/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.GoodsReceived == null)
            {
                return NotFound();
            }

            var goodsReceived = await _context.GoodsReceived.FindAsync(id);
            if (goodsReceived == null)
            {
                return NotFound();
            }
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "Name", goodsReceived.SupplierId);
            return View(goodsReceived);
        }

        // POST: GoodsReceiveds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GoodsReceivedId,ReceiptDate,TotalReceivedCost,SupplierId")] GoodsReceived goodsReceived)
        {
            if (id != goodsReceived.GoodsReceivedId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(goodsReceived);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GoodsReceivedExists(goodsReceived.GoodsReceivedId))
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
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "Name", goodsReceived.SupplierId);
            return View(goodsReceived);
        }

        // GET: GoodsReceiveds/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.GoodsReceived == null)
            {
                return NotFound();
            }

            var goodsReceived = await _context.GoodsReceived
                .Include(g => g.Supplier)
                .FirstOrDefaultAsync(m => m.GoodsReceivedId == id);
            if (goodsReceived == null)
            {
                return NotFound();
            }

            return View(goodsReceived);
        }

        // POST: GoodsReceiveds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.GoodsReceived == null)
            {
                return Problem("Entity set 'ApplicationDbContext.GoodsReceived'  is null.");
            }
            var goodsReceived = await _context.GoodsReceived.FindAsync(id);
            if (goodsReceived != null)
            {
                _context.GoodsReceived.Remove(goodsReceived);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GoodsReceivedExists(int id)
        {
          return (_context.GoodsReceived?.Any(e => e.GoodsReceivedId == id)).GetValueOrDefault();
        }
    }
}
