using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CassiniConnect.Core.Models.UserCore;
using CassiniConnect.Core.Utilities.Config;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace CassiniConnect.Application.UserManagement
{
    public class LoginUser
    {
        public class LoginUserCommand : IRequest<string>
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, string>
        {
            private readonly JwtSettings jwtSettings;
            private readonly SignInManager<User> signInManager;
            private readonly UserManager<User> userManager;
            public LoginUserCommandHandler(JwtSettings jwtSettings, SignInManager<User> signInManager, UserManager<User> userManager)
            {
                this.jwtSettings = jwtSettings;
                this.signInManager = signInManager;
                this.userManager = userManager;
            }

            public async Task<string> Handle(LoginUserCommand request, CancellationToken cancellation)
            {
                var user = await userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    throw new Exception("Érvénytelen email és jelszó kombináció!");
                }
                var result = await signInManager.PasswordSignInAsync(user, request.Password, false, false);
                if (!result.Succeeded)
                {
                    throw new Exception("Érvénytelen email és jelszó kombináció!");
                }

                return GenerateJwtToken(user);
            }

            private string GenerateJwtToken(User user)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

                if(user.Email == null)
                {
                    throw new Exception("Email nem található, hiba történt!");
                }

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(
                    [
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                ]),
                    Expires = DateTime.UtcNow.AddHours(2),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
        }
    }
}