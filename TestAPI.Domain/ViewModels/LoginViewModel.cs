using System.ComponentModel.DataAnnotations;

namespace TestAPI.Domain.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email can only contain letters, numbers, and special characters like ._%+-@")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Code is required.")]
    [StringLength(10, ErrorMessage = "Code cannot be longer than 20 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Code can only contain letters and numbers.")]
    [MinLength(3, ErrorMessage = "Code must be at least 3 characters long.")]
    public string Code { get; set; } = null!;
}
