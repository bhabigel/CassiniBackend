using CassiniConnect.Core.Models.UserCore;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CassiniConnect.Application.UserManagement
{
    public class RegisterUserCommand : IRequest<Guid>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }


    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
    {
        private readonly UserManager<User> userManager;

        public RegisterUserCommandHandler(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellation)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                UserName = request.Email
            };

            var result = await userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                throw new Exception($"Regisztráció sikertelen: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            return user.Id;
        }
    }
}