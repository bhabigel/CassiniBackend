using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CassiniConnect.Core.Models.Presentation
{
    public class Presenter
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("Users")]
        public Guid UserId { get; set; }
        public string Image { get; set; } = string.Empty;
        public ICollection<PresenterDetail> Details { get; set; } = [];
        public ICollection<PresenterBooking> Bookings { get; set; } = [];
    }
}