using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PSKCrackers.Data;
using PSKCrackers.Models;

namespace PSKCrackers.Controllers
{
    [Authorize]
    public class PendingOrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PendingOrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Sales
               .Where(u => !u.IsConfirmedOrder)
               .Include(s => s.Customer);
            return View(await applicationDbContext.ToListAsync());
        }

        [HttpGet]
        public object ConfirmOrder(string orderID)
        {
            bool hasError = false;
            string exceptionMsg = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(orderID))
                {
                    throw new Exception("OrderID number is required.");
                }

                Dictionary<SaleItem, string> _errors = new Dictionary<SaleItem, string>();


                bool orderIDStat = int.TryParse(orderID, out int _orderID);

                if (orderIDStat)
                {
                    // Search for the user in the simulated data
                    var Sale = _context.Sales
                        .Include(u => u.SaleItems)
                        .FirstOrDefault(u => u.SaleId == _orderID && !u.IsConfirmedOrder);

                    if (Sale != null)
                    {
                        foreach (var item in Sale.SaleItems)
                        {
                            var inventory = _context.InventoryItems.FirstOrDefault(u => u.ProductId == item.ProductId);
                            if (inventory != null)
                            {
                                if (inventory.QuantityInStock >= item.QuantityInCart)
                                {
                                    inventory.QuantityInStock -= item.QuantityInCart;
                                }
                                else
                                {
                                    //stock not available
                                    _errors.Add(item, "Stock Not Available");
                                }
                            }
                            else
                            {
                                //product not found
                                _errors.Add(item, "Product not found");
                            }
                        }

                        if (_errors.Any())
                        {
                            string errorMsg = string.Empty;
                            foreach (var item in _errors)
                            {
                                errorMsg += string.Format("{0} : {1}", item.Key.Product.Name, item.Value);
                                errorMsg += Environment.NewLine;
                            }
                            throw new Exception(errorMsg);
                        }
                        else
                        {
                            Sale.IsConfirmedOrder = true;

                            _context.SaveChanges();
                        }
                    }
                    else
                    {
                        //sale not found
                        throw new Exception("Sale not found.");
                    }
                }
                else
                {
                    //order ID is not valid
                    throw new Exception("Sale Id is not valid.");
                }
            }
            catch (Exception ex)
            {
                exceptionMsg = ex.Message;
                hasError = true;
            }

            var response = new { HasError = hasError, ExceptionMsg = exceptionMsg };

            return response;
        }
    }
}
