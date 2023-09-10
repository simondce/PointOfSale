namespace PSKCrackers.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;

    public class Product
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Product description is required.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Product barcode is required.")]
        public string Barcode { get; set; }

        [Required(ErrorMessage = "Product price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        // Foreign key for Supplier
        public int SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }

        // Navigation property for InventoryItems (items in stock)
        public virtual ICollection<InventoryItem> InventoryItems { get; set; }

        // Navigation property for PurchaseOrderItems (items in purchase orders)
        public virtual ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; }
    }

    public class Customer
    {
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Customer name is required.")]
        public string Name { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string PhoneNumber { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }

        // Navigation property for Sales (customer's sales)
        public virtual ICollection<Sale> Sales { get; set; }
    }

    public class Sale
    {
        public int SaleId { get; set; }

        [Required(ErrorMessage = "Sale date is required.")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
        [Display(Name = "Sale Date")]
        public DateTime SaleDate { get; set; }

        [Required(ErrorMessage = "Total amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be greater than 0.")]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        [Range(0, 100, ErrorMessage = "Discount percentage must be between 0 and 100.")]
        [Display(Name = "Discount (%)")]
        public decimal DiscountPercentage { get; set; }

        // Foreign key for Customer
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        // Navigation property for SaleItems (items in the sale)
        public virtual ICollection<SaleItem> SaleItems { get; set; }
    }

    public class SaleItem
    {
        public int SaleItemId { get; set; }

        [Required(ErrorMessage = "Product is required.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Unit Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit Price must be greater than 0.")]
        public decimal UnitPrice { get; set; }

        // Foreign key for Sale
        public int SaleId { get; set; }
        public virtual Sale Sale { get; set; }

        // Navigation property for Product
        public virtual Product Product { get; set; }
    }

    public class InventoryItem
    {
        public int InventoryItemId { get; set; }

        [Required(ErrorMessage = "Product is required.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity in stock is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a positive number.")]
        public int QuantityInStock { get; set; }

        [Required(ErrorMessage = "Last Restocked date is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Last Restocked")]
        public DateTime LastRestocked { get; set; }

        public virtual Product Product { get; set; }

        // Foreign key for InventoryLocation
        public int InventoryLocationId { get; set; }
        public virtual InventoryLocation InventoryLocation { get; set; }
    }

    public class InventoryLocation
    {
        public int InventoryLocationId { get; set; }

        [Required(ErrorMessage = "Location name is required.")]
        public string Name { get; set; }

        public string Description { get; set; }

        // Navigation property for InventoryItems (items in the location)
        public virtual ICollection<InventoryItem> InventoryItems { get; set; }
    }

    public class Supplier
    {
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "Supplier name is required.")]
        public string Name { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string PhoneNumber { get; set; }

        // Navigation property for Products supplied by the supplier
        public virtual ICollection<Product> SuppliedProducts { get; set; }

        // Navigation property for PurchaseOrders (supplier's purchase orders)
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }
    }

    public class PurchaseOrder
    {
        public int PurchaseOrderId { get; set; }

        [Required(ErrorMessage = "Order date is required.")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }

        [Required(ErrorMessage = "Total cost is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total cost must be greater than 0.")]
        [DataType(DataType.Currency)]
        public decimal TotalCost { get; set; }

        // Foreign key for Supplier
        public int SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }

        // Navigation property for PurchaseOrderItems (items in the order)
        public virtual ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; }
    }

    public class PurchaseOrderItem
    {
        public int PurchaseOrderItemId { get; set; }

        [Required(ErrorMessage = "Product is required.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Unit Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit Price must be greater than 0.")]
        public decimal UnitPrice { get; set; }

        // Foreign key for PurchaseOrder
        public int PurchaseOrderId { get; set; }
        public virtual PurchaseOrder PurchaseOrder { get; set; }

        // Navigation property for Product
        public virtual Product Product { get; set; }
    }


}
