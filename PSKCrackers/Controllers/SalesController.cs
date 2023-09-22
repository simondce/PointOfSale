using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PSKCrackers.Data;
using PSKCrackers.Helpers;
using PSKCrackers.Models;

namespace PSKCrackers.Controllers
{
    [Authorize]
    public class SalesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Sales
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Sales.Include(s => s.Customer);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Sales/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Sales == null)
            {
                return NotFound();
            }

            var sale = await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.SaleItems)
                .FirstOrDefaultAsync(m => m.SaleId == id);
            if (sale == null)
            {
                return NotFound();
            }

            ViewData["AvailableStock"] = _context.InventoryItems
                .Where(u => u.QuantityInStock > 0)
                .Include(p => p.Product).ToArray();

            return View(sale);
        }

        // GET: Sales/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "Name");
            ViewData["AvailableStock"] = _context.InventoryItems
                .Where(u=>u.QuantityInStock > 0)
                .Include(p => p.Product).ToArray();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Sale sale)
        {

            // Convert the Sale model into the appropriate database entities and save them to the database.
            // You'll need to map Sale and SaleItem to your database tables.

            // For example, if you have a SaleEntity and SaleItemEntity in your database context, you can do something like this:

            var saleEntity = new Sale
            {
                // Map properties from Sale to SaleEntity
                TotalAmount = sale.TotalAmount,
                DiscountPercentage = sale.DiscountPercentage,
                DiscountedTotal = sale.DiscountedTotal,
                SaleDate = DateTime.Now,
                CustomerId = sale.CustomerId
            };

            // Map SaleItem instances to SaleItemEntity instances and add them to the SaleEntity
            saleEntity.SaleItems = sale.SaleItems.Select(item => new SaleItem
            {
                // Map properties from SaleItem to SaleItemEntity
                ProductId = item.ProductId,
                QuantityInCart = item.QuantityInCart,
                Sale = saleEntity // Set the relationship to the parent sale
            }).ToList();

            _context.Sales.Add(saleEntity);

            foreach(var item in saleEntity.SaleItems)
            {
                var inventory = _context.InventoryItems.FirstOrDefault(u=>u.ProductId == item.ProductId);
                if(inventory != null)
                {
                    inventory.QuantityInStock -= item.QuantityInCart;
                }
            }


            await _context.SaveChangesAsync();

            return Ok(); // Sale saved successfully
        }

        // POST: Sales/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("SaleId,SaleDate,CustomerId,DiscountPercentage")] Sale sale)
        //{
        //    Utils.removeVirtualProperties(sale, ModelState);
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(sale);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "Name", sale.CustomerId);
        //    return View(sale);
        //}

        // GET: Sales/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Sales == null)
            {
                return NotFound();
            }

            var sale = await _context.Sales.FindAsync(id);
            if (sale == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "Name", sale.CustomerId);
            return View(sale);
        }

        // POST: Sales/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SaleId,SaleDate,CustomerId,DiscountPercentage")] Sale sale)
        {
            if (id != sale.SaleId)
            {
                return NotFound();
            }

            Utils.removeVirtualProperties(sale, ModelState);
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sale);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SaleExists(sale.SaleId))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "Name", sale.CustomerId);
            return View(sale);
        }

        // GET: Sales/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Sales == null)
            {
                return NotFound();
            }

            var sale = await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.SaleItems)
                .FirstOrDefaultAsync(m => m.SaleId == id);

            if (sale == null)
            {
                return NotFound();
            }

            ViewData["AvailableStock"] = _context.InventoryItems
              .Where(u => u.QuantityInStock > 0)
              .Include(p => p.Product).ToArray();

            return View(sale);
        }

        // POST: Sales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Sales == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Sales'  is null.");
            }
            var sale = await _context.Sales
                .Include(u=>u.SaleItems)
                .FirstOrDefaultAsync(u=>u.SaleId == id);


            foreach (var item in sale.SaleItems)
            {
                var inventory = _context.InventoryItems.FirstOrDefault(u => u.ProductId == item.ProductId);
                if (inventory != null)
                {
                    inventory.QuantityInStock += item.QuantityInCart;
                }
            }
            if (sale != null)
            {
                _context.Sales.Remove(sale);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SaleExists(int id)
        {
          return (_context.Sales?.Any(e => e.SaleId == id)).GetValueOrDefault();
        }
    }
}
