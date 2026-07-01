using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CourierManagementSystem.Models
{
    [Table("Customers")]
    public class Customer : Person
    {
        [Required] public string Country { get; set; } = "";
        [Required] public string Province { get; set; } = "";
        [Required] public string City { get; set; } = "";
        [Required] public string Area { get; set; } = "";
        [Required] public string Address { get; set; } = "";


        public virtual ICollection<Parcel> BookedParcels { get; set; } = new List<Parcel>();
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
