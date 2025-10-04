using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CassiniConnect.Core.Utilities.DTO
{
    [Serializable]
    public class SubjectNameDTO
    {
        public Guid LanguageId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
    }
}