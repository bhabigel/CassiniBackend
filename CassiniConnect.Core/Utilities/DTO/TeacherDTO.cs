using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CassiniConnect.Core.Utilities.DTO
{
    public class TeacherDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Subjects { get; set; } = [];

    }
}