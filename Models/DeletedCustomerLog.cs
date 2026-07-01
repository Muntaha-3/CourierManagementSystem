using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CourierManagementSystem.Models
{
    [Table("DeletedCustomerLogs")]
    public class DeletedCustomerLog
    {
        [Key]
        public int Id { get; set; }


        public int OriginalCustomerId { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string Address { get; set; } = "";


        // How many parcels/invoices were also deleted
        public int ParcelsDeleted { get; set; } = 0;
        public int InvoicesDeleted { get; set; } = 0;


        public DateTime DeletedAt { get; set; } = DateTime.Now;
        public string DeletedBy { get; set; } = "Admin";
    }
}
