using System.ComponentModel.DataAnnotations;

namespace TestAPI.Domain.ViewModels;

public class UserViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(20, ErrorMessage = "Name cannot be longer than 50 characters.")]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Name can only contain letters.")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Code is required.")]
    [StringLength(10, ErrorMessage = "Code cannot be longer than 20 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Code can only contain letters and numbers.")]
    [MinLength(3, ErrorMessage = "Code must be at least 3 characters long.")]
    public string Code { get; set; } = null!;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email can only contain letters, numbers, and special characters like ._%+-@")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Phone number is required.")]
    [StringLength(10, ErrorMessage = "Phone number must be exactly 10 digits long.")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits long.")]
    public string Phone { get; set; } = null!;

    [StringLength(100, ErrorMessage = "Address cannot be longer than 100 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9\s,.'-]+$", ErrorMessage = "Address can only contain letters, numbers, spaces, and special characters like ,.'-")]
    [Required(ErrorMessage = "Address is required.")]
    [MinLength(5, ErrorMessage = "Address must be at least 5 characters long.")]
    public string? Address { get; set; }

    [Required(ErrorMessage = "Nickname is required.")]
    [StringLength(20, ErrorMessage = "Nickname cannot be longer than 20 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Nickname can only contain letters and numbers.")]
    public string Nickname { get; set; } = null!;
}
