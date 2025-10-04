using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CassiniConnect.Core.Utilities.DTO
{
    [Serializable]
    public class TeacherDescriptionDTO
    {
        public string LanguageCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}