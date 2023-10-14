using System.ComponentModel.DataAnnotations;

namespace VarsityNexus.Models
{
    public class Institution
    {
        private string Name { get; set; } = string.Empty;
        [Required]
        private string Location { get; set; } = string.Empty;
        public Event hostEvent;
    }
}
