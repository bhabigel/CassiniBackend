using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CassiniConnect.Core.Models.Presentation
{
    public class PresenterBooking
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey(nameof(Presenter))]
        public Guid PresenterId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Booker { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public Presenter Presenter { get; set; } = null!;
    }
}