using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CassiniConnect.Core.Models.Presentation
{
    public class PresenterSession
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey(nameof(Booking))]
        public Guid BookingId { get; set; }
        public float Price { get; set; }
        public PresenterBooking Booking { get; set; } = null!;

    }
}