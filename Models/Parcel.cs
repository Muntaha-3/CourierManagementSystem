using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CourierManagementSystem.Models
{
    public enum ParcelStatus { Booked, InTransit, ArrivedAtHub, OutForDelivery, Delivered }
    public enum ServiceType { Standard, Express }


    [Table("Parcels")]
    public class Parcel
    {
        [Key]
        public int Id { get; set; } // This is the Parcel ID (#)


        public string TrackingNumber { get; set; } = "";


        // FK Linking to Customers table (C-)
        public int CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer? Sender { get; set; }


        public int? EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public virtual Employee? AssignedDriver { get; set; }


        [Required] public string ReceiverName { get; set; } = "";
        [Required][Phone] public string ReceiverPhone { get; set; } = "";


        // --- FIXED: Added these back so the Create page works ---
        [Required] public string ReceiverCountry { get; set; } = "";
        [Required] public string ReceiverProvince { get; set; } = "";
        [Required] public string ReceiverCity { get; set; } = "";
        [Required] public string ReceiverArea { get; set; } = "";
        [Required] public string ReceiverAddress { get; set; } = ""; // Street/House


        public string Description { get; set; } = "";
        public double Weight { get; set; }
        public ServiceType Service { get; set; } = ServiceType.Standard;


        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalCost { get; set; }


        public ParcelStatus Status { get; set; } = ParcelStatus.Booked;
        public string CurrentLocation { get; set; } = "Origin Office";
        public DateTime BookingDate { get; set; } = DateTime.Now;


        public virtual Invoice? Invoice { get; set; }


        public Parcel()
        {
            TrackingNumber = "TRK-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        }
    }
}
