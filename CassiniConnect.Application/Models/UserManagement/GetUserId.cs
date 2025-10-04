using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Core.Models.UserCore;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CassiniConnect.Application.Models.UserManagement
{
    public class GetUserId
    {
        public class GetUserIdRequest : IRequest<Guid>
        {
            public string Email { get; set; } = string.Empty;
        }

        public class GetUserIdRequestHandler : IRequestHandler<GetUserIdRequest, Guid>
        {
            private readonly UserManager<User> userManager;
            public GetUserIdRequestHandler(UserManager<User> userManager)
            {
                this.userManager = userManager;
            }

            public async Task<Guid> Handle(GetUserIdRequest request, CancellationToken cancellationToken)
            {
                if(string.IsNullOrEmpty(request.Email))
                {
                    throw new Exception("Email given is empty or null!");
                }

                var user = await userManager.FindByEmailAsync(request.Email);
                if(user == null)
                {
                    throw new Exception("User was not found by this email!");
                }

                return user.Id;
            }
        }
    }
}