using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CassiniConnect.Core.Models.Teaching
{
    public class Subject
    {
        [Key]
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public ICollection<Teacher> Teachers { get; set; } = [];
        public ICollection<SubjectName> SubjectNames { get; set; } = [];
    }
}