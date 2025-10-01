using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Core.Models.Helpers;

namespace CassiniConnect.Core.Models.Presentation
{
    public class PresenterDetail
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey(nameof(Presenter))]
        public Guid PresenterId { get; set; }
        [ForeignKey(nameof(Language))]
        public Guid LanguageId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Themes { get; set; } = string.Empty;
        public Presenter Presenter { get; set; } = null!;
        public LanguageCode Language { get; set; } = null!;
    }
}