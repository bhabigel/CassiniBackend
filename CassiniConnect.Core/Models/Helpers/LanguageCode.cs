using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CassiniConnect.Core.Models.Helpers
{
    public class LanguageCode
    {
        [Key]
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
    }
}