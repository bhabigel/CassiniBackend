using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Core.Models.Helpers;

namespace CassiniConnect.Core.Models.Teaching
{
    public class TeacherDescription
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey(nameof(TeacherId))]
        public Guid TeacherId { get; set; }
        [ForeignKey(nameof(LanguageId))]
        public Guid LanguageId { get; set; }
        public string Description { get; set; } = string.Empty;
        public LanguageCode Language { get; set; } = null!;
        public Teacher Teacher { get; set; } = null!;
    }
}