using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CassiniConnect.Core.Models.Teaching.Group
{
    public class GroupActivity
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey(nameof(Teacher))]
        public Guid TeacherId { get; set; }
        public float Price { get; set; }
        public Teacher Teacher { get; set; } = null!;
        public ICollection<GroupActivityDetail> GroupActivityDetails = []; 
    }
}