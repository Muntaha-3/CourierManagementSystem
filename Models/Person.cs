using System.ComponentModel.DataAnnotations;


namespace CourierManagementSystem.Models
{
    public abstract class Person
    {
        [Key]
        public int Id { get; set; }


        [Required]
        public string Name { get; set; } = "";


        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";


        [Required]
        // This ensures the number starts with 03 and has 11 digits
        [RegularExpression(@"^[0]3\d{9}$", ErrorMessage = "Invalid format. Must start with 03 and be 11 digits.")]
        public string PhoneNumber { get; set; } = "";
    }
}
