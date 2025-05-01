using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class LoginViewModel
{
    [Required]
    [RegularExpression("^$", ErrorMessage = "Invalid email")]
    [DataType(DataType.EmailAddress)]
    [Display(Name = "Email", Prompt = "Enter email")]

    public string Email { get; set; } = null!;

    [Required]
    [RegularExpression("^$", ErrorMessage = "Invalid password")]
    [DataType(DataType.Password)]
    [Display(Name = "Password", Prompt = "Enter password")]

    public string Password { get; set; } = null!;

    public bool RemeberMe { get; set; }
}
