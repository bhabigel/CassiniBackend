using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CassiniConnect.Core.Models.Teaching
{
    public class TeachingLocation
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set;} = string.Empty;
    }
}