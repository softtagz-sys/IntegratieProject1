using System.ComponentModel.DataAnnotations;

namespace net6npmwebpack.Models;

public class ContactViewModel
{
    [Required]
    [StringLength(30, MinimumLength = 3)]
    public string Subject { get; set; }

    [Required(ErrorMessage = "Please enter a message.")]
    public string Message { get; set; }
}