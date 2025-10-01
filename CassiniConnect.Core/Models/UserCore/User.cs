using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Core.Models.Presentation;
using CassiniConnect.Core.Models.Teaching;
using CassiniConnect.Core.Models.Teaching.Group;
using Microsoft.AspNetCore.Identity;

namespace CassiniConnect.Core.Models.UserCore
{
    public class User : IdentityUser<Guid>
    {
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public ICollection<Teacher> Teachers { get; set; } = [];
        public ICollection<Presenter> Presenters { get; set; } = [];
        public ICollection<TutoringAppointment> TutoringAppointments { get; set; } = [];
        public ICollection<GroupSession> GroupSessions { get; set; } = [];
    }
}