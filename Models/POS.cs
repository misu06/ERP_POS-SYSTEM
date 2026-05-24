using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public class CartItem
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string Barcode { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal => Price * Quantity;
    }

    public class SaleTransaction
    {
        public int TransactionId { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime TransactionDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string PaymentMethod { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal ChangeDue { get; set; }
        public List<CartItem> Items { get; set; }
        public string CashierName { get; set; }
        public string Status { get; set; }
    }

    public class PaymentDetails
    {
        public string Method { get; set; } // Cash, Card, Mobile Payment, Gift Card
        public decimal AmountPaid { get; set; }
        public string CardNumber { get; set; } // For card payments
        public string CardHolderName { get; set; }
        public string ExpiryDate { get; set; }
        public string MobileNumber { get; set; } // For mobile payments
        public string TransactionId { get; set; }
    }

    public class POSSettings
    {
        public decimal TaxRate { get; set; } = 0.08m; // 8% default tax
        public string CurrencySymbol { get; set; } = "$";
        public bool EnableCustomerLoyalty { get; set; } = true;
        public bool EnableBarcodeScanning { get; set; } = true;
        public int DefaultDiscountPercentage { get; set; } = 0;
    }

    public class SaleResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal ChangeDue { get; set; }
    }
}