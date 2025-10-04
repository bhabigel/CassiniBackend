using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CassiniConnect.Core.Utilities.DTO
{
    [Serializable]
    public class PresenterDetailDTO
    {
        public Guid LanguageId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Themes { get; set; } = string.Empty;
    }
}