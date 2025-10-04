using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Core.Models.Helpers;

namespace CassiniConnect.Core.Models.Teaching
{
    public class SubjectName
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey(nameof(Subject))]
        public Guid SubjectId { get; set; }
        [ForeignKey(nameof(Language))]
        public Guid LanguageId { get; set; }
        public string Name { get; set; } = string.Empty;
        public LanguageCode Language { get; set; } = null!;
        public Subject Subject { get; set; } = null!;
    }
}