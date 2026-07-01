using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CourierManagementSystem.Models
{
    public enum InvoiceStatus { Draft, Sent, Paid, Overdue, Cancelled }
    public enum PaymentMethod { Cash, BankTransfer, CreditCard, EasyPaisa, JazzCash }


    [Table("Invoices")]
    public class Invoice
    {
        [Key]
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = "";
        public int ParcelId { get; set; }
        [ForeignKey("ParcelId")]
        public virtual Parcel? Parcel { get; set; }


        // Linked Customer
        public int CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        public decimal ParcelBaseAmount { get; set; }
        public double GstPercent { get; set; } = 17.0;
        [Column(TypeName = "decimal(18,2)")]
        public decimal GstAmount { get; set; }
        public double AdditionalTaxPercent { get; set; } = 0.0;
        [Column(TypeName = "decimal(18,2)")]
        public decimal AdditionalTaxAmount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal FuelSurcharge { get; set; } = 0;
        [Column(TypeName = "decimal(18,2)")]
        public decimal RemoteAreaSurcharge { get; set; } = 0;
        [Column(TypeName = "decimal(18,2)")]
        public decimal InsuranceFee { get; set; } = 0;
        public double DiscountPercent { get; set; } = 0.0;
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }
        public string? GiftCode { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal GiftCodeDiscount { get; set; } = 0;


        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalTax { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalDiscount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal GrandTotal { get; set; }


        public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;


        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountPaid { get; set; } = 0;
        [Column(TypeName = "decimal(18,2)")]
        public decimal BalanceDue { get; set; }
        public DateTime IssueDate { get; set; } = DateTime.Now;
        public DateTime DueDate { get; set; } = DateTime.Now.AddDays(7);
        public string? Notes { get; set; }


        public Invoice()
        {
            InvoiceNumber = "INV-" + DateTime.Now.ToString("yyyyMMdd") + "-" + Guid.NewGuid().ToString().Substring(0, 4).ToUpper();
        }
    }
}
