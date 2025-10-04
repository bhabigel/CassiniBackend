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

namespace CassiniConnect.Application.Models.UserManagement
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
            private readonly RoleManager<Role> roleManager;
            public LoginUserCommandHandler(JwtSettings jwtSettings, SignInManager<User> signInManager, UserManager<User> userManager, RoleManager<Role> roleManager)
            {
                this.jwtSettings = jwtSettings;
                this.signInManager = signInManager;
                this.userManager = userManager;
                this.roleManager = roleManager;
            }

            public async Task<string> Handle(LoginUserCommand request, CancellationToken cancellation)
            {
                var user = await userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    throw new Exception("Invalid combination of email and password!");
                }
                var result = await signInManager.PasswordSignInAsync(user, request.Password, false, false);
                if (!result.Succeeded)
                {
                    throw new Exception("Invalid combination of email and password!");
                }

                return await GenerateJwtToken(user);
            }

            private async Task<string> GenerateJwtToken(User user)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

                if (user.Email == null)
                {
                    throw new Exception("Email not found!");
                }
                var roles = await userManager.GetRolesAsync(user);

                if (roles == null)
                {
                    throw new Exception("No role found!");
                }

                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                    new Claim(JwtRegisteredClaimNames.Iss, jwtSettings.Issuer),
                    new Claim(JwtRegisteredClaimNames.Aud, jwtSettings.Audience)
                };

                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddHours(2),
                    Issuer = jwtSettings.Issuer,
                    Audience = jwtSettings.Audience,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
        }
    }
}