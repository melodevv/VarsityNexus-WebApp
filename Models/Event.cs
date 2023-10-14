using System.ComponentModel.DataAnnotations;

namespace VarsityNexus.Models
{
    public class Event
    {
        [DataType(DataType.Date)]
        private DateTime Date { get; set; }
        public int EventId { get; set; }
        [Required]
        public string EventType { get; set; }
    }
}
