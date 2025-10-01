using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Core.Models.UserCore;

namespace CassiniConnect.Core.Models.Teaching.Group
{
    public class GroupSession
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey(nameof(GroupActivity))]
        public Guid ActivityId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public GroupActivity GroupActivity  { get; set; } = null!;
        public ICollection<User> Participants { get; set; } = [];
    }
}