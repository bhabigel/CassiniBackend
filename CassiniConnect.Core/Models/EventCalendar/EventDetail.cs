using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Core.Models.Helpers;

namespace CassiniConnect.Core.Models.EventCalendar
{
    public class EventDetail
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey(nameof(Event))]
        public Guid EventId { get; set; }
        [ForeignKey(nameof(Language))]
        public Guid LanguageId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public Event Event { get; set; } = null!;
        public LanguageCode Language { get; set; } = null!;
    }
}