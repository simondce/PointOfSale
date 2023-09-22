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
    public class PurchaseOrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PurchaseOrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PurchaseOrders
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.PurchaseOrders.Include(p => p.Supplier);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> AddToInventory(int? id)
        {
            if (id == null || _context.PurchaseOrders == null)
            {
                return NotFound();
            }

            var purchaseOrder = await _context.PurchaseOrders
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseOrderItems)
                .FirstOrDefaultAsync(m => m.PurchaseOrderId == id);

            purchaseOrder.PurchaseOrderItems.ForEach(u =>
            {
                if (_context.InventoryItems.Any(x => x.ProductId == u.ProductId))
                {
                    _context.InventoryItems.FirstOrDefault(x => x.ProductId == u.ProductId).QuantityInStock += u.Quantity;
                }
                else
                {
                    _context.InventoryItems.Add(new InventoryItem()
                    {
                        ProductId = u.ProductId,
                        QuantityInStock = u.Quantity
                    });
                }
            });

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // GET: PurchaseOrders/Details/5
        public async Task<IActionResult> ManagePO(int? id)
        {
            if (id == null || _context.PurchaseOrders == null)
            {
                return NotFound();
            }

            var purchaseOrder = await _context.PurchaseOrders
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseOrderItems)
                .FirstOrDefaultAsync(m => m.PurchaseOrderId == id);

            purchaseOrder.Supplier.SuppliedProducts = null;

            if (purchaseOrder == null)
            {
                return NotFound();
            }

            ViewData["ProductId"] = new SelectList(_context.Products.Where(u => u.SupplierId == purchaseOrder.SupplierId), "ProductId", "Name");

            return View(purchaseOrder);
        }

        [HttpPost]
        public ActionResult ManagePO(PurchaseOrder _TableForm)
        {

            var purchaseOrder = _context.PurchaseOrders
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseOrderItems)
                .FirstOrDefault(m => m.PurchaseOrderId == _TableForm.PurchaseOrderId);

            purchaseOrder.PurchaseOrderItems = new List<PurchaseOrderItem>();
            decimal totalCost = 0;
            //Loop through the forms
            for (int i = 0; i <= Request.Form.Count; i++)
            {
                var Product = Request.Form["Product[" + i + "]"];
                var Quantity = Request.Form["Quantity[" + i + "]"];


                if (Product.Count > 0 && Product.First() != null && Quantity.Count > 0 && Quantity.First() != null)
                {
                    if (purchaseOrder.PurchaseOrderItems.Any(u => u.ProductId == int.Parse(Product.First())))
                    {
                        ModelState.AddModelError("", "Duplicate Products selected");
                        ViewData["ProductId"] = new SelectList(_context.Products.Where(u => u.SupplierId == purchaseOrder.SupplierId), "ProductId", "Name");

                        return View(purchaseOrder);
                    }
                    purchaseOrder.PurchaseOrderItems.Add(new PurchaseOrderItem
                    {
                        ProductId = int.Parse(Product.First()),
                        Quantity = int.Parse(Quantity.First())
                    });

                    totalCost += (_context.Products.FirstOrDefault(u => u.ProductId == int.Parse(Product)).Price * int.Parse(Quantity));
                    //_TableForm.Add(new purchaseor { ClientSampleID = ClientSampleID, AcidStables = acidStables, AdditionalComments = additionalComments });
                }
            }
            purchaseOrder.TotalOrderCost = totalCost;

            _context.SaveChanges();

            ViewData["ProductId"] = new SelectList(_context.Products.Where(u => u.SupplierId == purchaseOrder.SupplierId), "ProductId", "Name");

            return View(purchaseOrder);
        }

        // GET: PurchaseOrders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.PurchaseOrders == null)
            {
                return NotFound();
            }

            var purchaseOrder = await _context.PurchaseOrders
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.PurchaseOrderId == id);
            if (purchaseOrder == null)
            {
                return NotFound();
            }

            return View(purchaseOrder);
        }

        // GET: PurchaseOrders/Create
        public IActionResult Create()
        {

            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "Name");
            return View();
        }

        // POST: PurchaseOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PurchaseOrderId,OrderDate,TotalOrderCost,SupplierId")] PurchaseOrder purchaseOrder)
        {
            Utils.removeVirtualProperties(purchaseOrder, ModelState);
            if (ModelState.IsValid)
            {
                _context.Add(purchaseOrder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "Name", purchaseOrder.SupplierId);
            return View(purchaseOrder);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkUpdate(PurchaseOrder purchaseOrder)
        {
            Utils.removeVirtualProperties(purchaseOrder, ModelState);
            //if (ModelState.IsValid)
            {
                _context.Add(purchaseOrder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManagePO), purchaseOrder.PurchaseOrderId);
            }
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "Name", purchaseOrder.SupplierId);
            return View(purchaseOrder);
        }

        // GET: PurchaseOrders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.PurchaseOrders == null)
            {
                return NotFound();
            }

            var purchaseOrder = await _context.PurchaseOrders.FindAsync(id);
            if (purchaseOrder == null)
            {
                return NotFound();
            }
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "Name", purchaseOrder.SupplierId);
            return View(purchaseOrder);
        }

        // POST: PurchaseOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PurchaseOrderId,OrderDate,TotalOrderCost,SupplierId")] PurchaseOrder purchaseOrder)
        {
            if (id != purchaseOrder.PurchaseOrderId)
            {
                return NotFound();
            }

            Utils.removeVirtualProperties(purchaseOrder, ModelState);
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(purchaseOrder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PurchaseOrderExists(purchaseOrder.PurchaseOrderId))
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
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "Name", purchaseOrder.SupplierId);
            return View(purchaseOrder);
        }

        // GET: PurchaseOrders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.PurchaseOrders == null)
            {
                return NotFound();
            }

            var purchaseOrder = await _context.PurchaseOrders
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.PurchaseOrderId == id);
            if (purchaseOrder == null)
            {
                return NotFound();
            }

            return View(purchaseOrder);
        }

        // POST: PurchaseOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.PurchaseOrders == null)
            {
                return Problem("Entity set 'ApplicationDbContext.PurchaseOrders'  is null.");
            }
            var purchaseOrder = await _context.PurchaseOrders.FindAsync(id);
            if (purchaseOrder != null)
            {
                _context.PurchaseOrders.Remove(purchaseOrder);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PurchaseOrderExists(int id)
        {
            return (_context.PurchaseOrders?.Any(e => e.PurchaseOrderId == id)).GetValueOrDefault();
        }
    }
}
