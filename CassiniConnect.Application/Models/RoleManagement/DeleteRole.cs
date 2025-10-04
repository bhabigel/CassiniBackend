using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Core.Models.UserCore;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CassiniConnect.Application.Models.RoleManagement
{
    public class DeleteRole
    {
        public class DeleteRoleCommand : IRequest<Unit>
        {
            public string RoleName { get; set; } = string.Empty;
        }

        public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, Unit>
        {
            private readonly RoleManager<Role> roleManager;
            public DeleteRoleCommandHandler(RoleManager<Role> roleManager)
            {
                this.roleManager = roleManager;
            }

            public async Task<Unit> Handle(DeleteRoleCommand command, CancellationToken cancellationToken)
            {
                if (string.IsNullOrEmpty(command.RoleName))
                {
                    throw new Exception("Name of role is empty!");
                }

                var role = await roleManager.FindByNameAsync(command.RoleName);
                if (role == null)
                {
                    throw new Exception($"Role not found with name {command.RoleName}");
                }

                var result = await roleManager.DeleteAsync(role);
                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to delete role: {string.Join(", ", result.Errors)}");
                }

                return Unit.Value;
            }
        }
    }
}