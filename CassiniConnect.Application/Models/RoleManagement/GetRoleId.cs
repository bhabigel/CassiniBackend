using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Core.Models.UserCore;
using CassiniConnect.Core.Persistance;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CassiniConnect.Application.Models.RoleManagement
{
    public class GetRoleId
    {
        public class GetRoleIdRequest : IRequest<Guid>
        {
            public string RoleName { get; set; } = string.Empty;
        }

        public class GetRoleIdRequestHandler : IRequestHandler<GetRoleIdRequest, Guid>
        {
            private readonly RoleManager<Role> roleManager;
            public GetRoleIdRequestHandler(RoleManager<Role> roleManager)
            {
                this.roleManager = roleManager;
            }

            public async Task<Guid> Handle(GetRoleIdRequest request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrEmpty(request.RoleName))
                {
                    throw new Exception("Role name given is empty or null!");
                }

                var roleId = await roleManager.Roles.Where(r => r.Name == request.RoleName).Select(r => r.Id).FirstOrDefaultAsync();
                return roleId;
            }
        }
    }
}