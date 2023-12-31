﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PSKCrackers.Data;
using PSKCrackers.Helpers;
using PSKCrackers.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Data;
using System.Reflection;

namespace PSKCrackers.Controllers
{
    [Authorize]
    public class InventoryItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InventoryItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: InventoryItems
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.InventoryItems.Include(i => i.Product).Include(i => i.Product.Supplier);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: InventoryItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.InventoryItems == null)
            {
                return NotFound();
            }

            var inventoryItem = await _context.InventoryItems
                .Include(i => i.Product)
                .FirstOrDefaultAsync(m => m.InventoryItemId == id);
            if (inventoryItem == null)
            {
                return NotFound();
            }

            return View(inventoryItem);
        }

        // GET: InventoryItems/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name");
            return View();
        }

        // POST: InventoryItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InventoryItemId,ProductId,QuantityInStock")] InventoryItem inventoryItem)
        {
            Utils.removeVirtualProperties(inventoryItem, ModelState);
            if (ModelState.IsValid)
            {
                _context.Add(inventoryItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name", inventoryItem.ProductId);
            return View(inventoryItem);
        }

        // GET: InventoryItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.InventoryItems == null)
            {
                return NotFound();
            }

            var inventoryItem = await _context.InventoryItems.FindAsync(id);
            if (inventoryItem == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name", inventoryItem.ProductId);
            return View(inventoryItem);
        }

        // POST: InventoryItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InventoryItemId,ProductId,QuantityInStock")] InventoryItem inventoryItem)
        {
            if (id != inventoryItem.InventoryItemId)
            {
                return NotFound();
            }

            Utils.removeVirtualProperties(inventoryItem, ModelState);
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inventoryItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InventoryItemExists(inventoryItem.InventoryItemId))
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
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name", inventoryItem.ProductId);
            return View(inventoryItem);
        }

        // GET: InventoryItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.InventoryItems == null)
            {
                return NotFound();
            }

            var inventoryItem = await _context.InventoryItems
                .Include(i => i.Product)
                .FirstOrDefaultAsync(m => m.InventoryItemId == id);
            if (inventoryItem == null)
            {
                return NotFound();
            }

            return View(inventoryItem);
        }

        // POST: InventoryItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.InventoryItems == null)
            {
                return Problem("Entity set 'ApplicationDbContext.InventoryItems'  is null.");
            }
            var inventoryItem = await _context.InventoryItems.FindAsync(id);
            if (inventoryItem != null)
            {
                _context.InventoryItems.Remove(inventoryItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InventoryItemExists(int id)
        {
            return (_context.InventoryItems?.Any(e => e.InventoryItemId == id)).GetValueOrDefault();
        }

        // Action for exporting data to Excel
        [HttpPost]
        public ActionResult ExportToExcel()
        {
            string tmpPath = Path.GetTempFileName();

            var _data = _context.InventoryItems.OrderByDescending(u=>u.QuantityInStock).ToList();

            CreateExcelFile(_data, tmpPath);

            // Generate the Excel file
            byte[] excelBytes = System.IO.File.ReadAllBytes(tmpPath);

            // Return the Excel file as a download
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Inventory.xlsx");
        }

        public void CreateExcelFile(List<InventoryItem> data, string filePath)
        {
            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();
                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();

                Workbook workbook = new Workbook();
                Worksheet worksheet = new Worksheet();
                SheetData sheetData = new SheetData();

                var objectType = data[0].GetType();
                PropertyInfo[] properties = objectType.GetProperties();

                // Create headers dynamically from the type of the first object in the list
                if (data.Count > 0)
                {
                    Row headerRow = new Row();
                    foreach (PropertyInfo property in properties)
                    {
                        headerRow.Append(CreateCell(property.Name));
                    }
                    sheetData.AppendChild(headerRow);
                }

                foreach (var item in data)
                {
                    Row dataRow = new Row();
                    foreach (PropertyInfo property in properties)
                    {
                        object value = property.GetValue(item);
                        dataRow.Append(CreateCell(value != null ? value.ToString() : string.Empty));
                    }
                    sheetData.AppendChild(dataRow);
                }

                worksheet.AppendChild(sheetData);

                Sheets sheets = new Sheets();
                Sheet sheet = new Sheet() { Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Sheet1" };
                sheets.Append(sheet);
                workbook.Append(sheets);

                workbookPart.Workbook = workbook;
                workbookPart.Workbook.Save();

            }
        }

        private Cell CreateCell(string text)
        {
            return new Cell(new InlineString(new Text(text)));
        }
    }
}
