using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Application.Models.UserManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CassiniConnect.API.Controllers
{
    public class UserController : BaseController
    {
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromForm] string firstName, [FromForm] string lastName, [FromForm] string email, [FromForm] string password, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return BadRequest("Some or more of obligatory fields are empty!");
            }

            var emailAttribute = new EmailAddressAttribute();
            if (!emailAttribute.IsValid(email))
            {
                return BadRequest("Invalid email format!");
            }

            try
            {
                await Mediator.Send(new RegisterUser.RegisterUserCommand
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    Password = password
                }, cancellationToken);

                return Ok("User registered successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> LoginUser([FromForm] string email, [FromForm] string password, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return BadRequest("Email or password is empty!");
            }

            try
            {
                var jwt = await Mediator.Send(new LoginUser.LoginUserCommand
                {
                    Email = email,
                    Password = password
                }, cancellationToken);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddHours(2)
                };

                Response.Cookies.Append("authToken", jwt, cookieOptions);
                return Ok(jwt);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("add-role")]
        public async Task<IActionResult> AddRoleToUser([FromForm] string email, [FromForm] string roleName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(roleName))
            {
                return BadRequest("Some or more of obligatory fields are empty!");
            }

            var emailAttribute = new EmailAddressAttribute();
            if (!emailAttribute.IsValid(email))
            {
                return BadRequest("Invalid email format!");
            }

            try
            {
                await Mediator.Send(new AddRoleToUser.AddRoleToUserCommand { Email = email, RoleName = roleName }, cancellationToken);
                return Ok("Role added to user successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }
    }

}