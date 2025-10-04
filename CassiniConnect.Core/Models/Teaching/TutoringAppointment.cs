using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Core.Models.UserCore;

namespace CassiniConnect.Core.Models.Teaching
{
    public class TutoringAppointment
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        [ForeignKey(nameof(Teacher))]
        public Guid TeacherId { get; set; }
        [ForeignKey(nameof(Subject))]
        public Guid SubjectId { get; set; }
        [ForeignKey(nameof(Location))]
        public Guid LocationId { get; set; }
        public User User { get; set; } = null!;
        public Teacher Teacher { get; set; } = null!;
        public Subject Subject { get; set; } = null!;
        public TeachingLocation Location { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TutoringSession? TutoringSession { get; set; }
    }
}