using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Core.Models.UserCore;
using CassiniConnect.Core.Persistance;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CassiniConnect.Application.Models.RoleManagement
{
    public class AddRole
    {
        public class AddRoleCommand : IRequest<Unit>
        {
            public string RoleName { get; set; } = string.Empty;
        }

        public class AddRoleCommandHandler : IRequestHandler<AddRoleCommand, Unit>
        {
            private readonly RoleManager<Role> roleManager;
            public AddRoleCommandHandler(RoleManager<Role> roleManager)
            {
                this.roleManager = roleManager;
            }

            public async Task<Unit> Handle(AddRoleCommand command, CancellationToken cancellationToken)
            {
                if (string.IsNullOrEmpty(command.RoleName))
                {
                    throw new Exception("Name of role is empty!");
                }

                var existingRole = await roleManager.FindByNameAsync(command.RoleName);
                if (existingRole != null)
                {
                    throw new InvalidOperationException("Role already exists!");
                }

                var role = new Role
                {
                    Id = Guid.NewGuid(),
                    Name = command.RoleName,
                };

                var result = await roleManager.CreateAsync(role);
                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to create role: {string.Join(", ", result.Errors)}");
                }

                return Unit.Value;
            }
        }
    }
}