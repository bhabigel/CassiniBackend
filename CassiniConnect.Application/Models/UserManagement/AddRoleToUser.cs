using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Core.Models.UserCore;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CassiniConnect.Application.Models.UserManagement
{
    public class AddRoleToUser
    {
        public class AddRoleToUserCommand : IRequest<Unit>
        {
            public string Email { get; set; } = string.Empty;
            public string RoleName { get; set; } = string.Empty;
        }

        public class AddRoleToUserCommandHandler : IRequestHandler<AddRoleToUserCommand, Unit>
        {
            private readonly UserManager<User> userManager;

            public AddRoleToUserCommandHandler(UserManager<User> userManager)
            {
                this.userManager = userManager;
            }

            public async Task<Unit> Handle(AddRoleToUserCommand command, CancellationToken cancellationToken)
            {
                if (string.IsNullOrEmpty(command.Email) || string.IsNullOrEmpty(command.RoleName))
                {
                    throw new Exception("One or more of the obligatory fields are empty!");
                }

                var user = await userManager.FindByEmailAsync(command.Email);
                if (user == null)
                {
                    throw new Exception("User was not found by given email!");
                }

                var result = await userManager.AddToRoleAsync(user, command.RoleName);
                if(!result.Succeeded)
                {
                    throw new Exception($"Adding role to user failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }

                return Unit.Value;
            }
        }
    }
}