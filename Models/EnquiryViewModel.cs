using System.ComponentModel.DataAnnotations;

namespace SJBRAssociatesWeb.Models;

public class EnquiryViewModel
{
    [Required(ErrorMessage = "Please enter your name")]
    [StringLength(100)]
    [Display(Name = "Your Name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your email")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your phone number")]
    [Phone(ErrorMessage = "Please enter a valid phone number")]
    [Display(Name = "Phone Number")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter a message")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "Message should be between 10 and 2000 characters")]
    public string Message { get; set; } = string.Empty;
}
