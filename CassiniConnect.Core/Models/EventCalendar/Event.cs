using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Core.Models.UserCore;

namespace CassiniConnect.Core.Models.EventCalendar
{
    public class Event
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey(nameof(Creator))]
        public Guid CreatorId { get; set; }
        public DateTime Date { get; set; }
        public User Creator { get; set; } = null!;
        public ICollection<EventDetail> EventDetails { get; set; } = [];
    }
}