using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CassiniConnect.Core.Models.UserCore
{
    public class Role : IdentityRole<Guid>
    {
        public ICollection<User> Users { get; set; } = [];
    }
}