using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CourierManagementSystem.Models
{
    // TPT: EF Core creates a separate Admins table joined to Persons on Id
    [Table("Admins")]
    public class Admin : Person
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";
    }
}
