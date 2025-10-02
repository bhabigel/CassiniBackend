using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CassiniConnect.Core.Utilities.DTO
{
    public class TeacherDescriptionDTO
    {
        public Guid LanguageId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}