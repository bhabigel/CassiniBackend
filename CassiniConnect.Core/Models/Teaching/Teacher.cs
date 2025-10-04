using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Core.Models.UserCore;

namespace CassiniConnect.Core.Models.Teaching
{
    public class Teacher
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        public float Rate { get; set; }
        public string Image { get; set; } = string.Empty;
        public User User { get; set; } = null!;
        public ICollection<Subject> Subjects { get; set; } = [];
        public ICollection<TutoringAppointment> TutoringAppointments { get; set; } = [];
        public ICollection<TeacherDescription> TeacherDescriptions { get; set; } = [];
    }
}