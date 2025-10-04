using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Core.Models.UserCore;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CassiniConnect.Application.Models.RoleManagement
{
    public class ListRoles
    {
        public class ListRolesRequest : IRequest<List<string>>
        {
        }

        public class ListRolesRequestHandler : IRequestHandler<ListRolesRequest, List<string>>
        {
            private readonly RoleManager<Role> roleManager;
            public ListRolesRequestHandler(RoleManager<Role> roleManager)
            {
                this.roleManager = roleManager;
            }

            public async Task<List<string>> Handle(ListRolesRequest request, CancellationToken cancellationToken)
            {
                var roles = await roleManager.Roles.Select(r => r.Name).Where(name => name != null).Select(name => name!).ToListAsync(cancellationToken);
                if(roles == null)
                {
                    throw new Exception("No roles were found!");
                }
                return roles;
            }
        }
    }
}