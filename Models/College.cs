using System.ComponentModel.DataAnnotations;

namespace VarsityNexus.Models
{
    public class College:Institution
    {
        [DataType(DataType.Date)]
        private DateTime Date{ get; set; }

        public int EventId { get;set; }
        [Required]
        public string EventType { get; set; }

        public Event hostEvent;
    }
}
