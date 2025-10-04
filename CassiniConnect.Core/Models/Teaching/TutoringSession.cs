using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CassiniConnect.Core.Models.Teaching
{
    public class TutoringSession
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey(nameof(Appointment))]
        public Guid AppointmentId { get; set; }
        public TutoringAppointment Appointment  { get; set; } = null!;
        public float Price { get; set; }
    }
}