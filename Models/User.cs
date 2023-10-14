using System.ComponentModel.DataAnnotations;

namespace VarsityNexus.Models
{
    public class User
    {
        [StringLength(20, MinimumLength = 3)]
        private string FirstName { get; set; } = string.Empty;
        [StringLength(20, MinimumLength = 3)]
        private string LastName { get; set; } = string.Empty;
        [Required]
        private string Email { get; set; } = string.Empty;
        private string Username { get; set; }
        [Required]
        private int MobileNumber { get;set; }
        private List<string> interests;
    }
}
