using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CourierManagementSystem.Models
{
    [Table("Employees")]
    public class Employee : Person
    {
        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; } = "";


        // NEW: Vehicle Type Column
        public string? VehicleType { get; set; }


        // Pattern: 3 Letters - 4 Numbers
        [RegularExpression(@"^[A-Z]{3}-\d{4}$", ErrorMessage = "Format: 3 Letters-4 Digits (e.g. LEA-1234)")]
        public string? VehicleNumber { get; set; }


        public string? AssignedArea { get; set; }


        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";


        public bool IsActive { get; set; } = true;


        public virtual ICollection<Parcel> AssignedParcels { get; set; } = new List<Parcel>();
    }
}
