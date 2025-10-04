using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Application.Models.RoleManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CassiniConnect.API.Controllers
{
    public class RoleController : BaseController
    {
        [Authorize(Roles = "Admin")]
        [HttpPost("add")]
        public async Task<IActionResult> AddRole(string roleName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return BadRequest("Role name is empty!");
            }

            try
            {
                await Mediator.Send(new AddRole.AddRoleCommand { RoleName = roleName }, cancellationToken);
                return Ok("Role addedd successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteRole(string roleName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return BadRequest("Role name is empty!");
            }

            try
            {
                await Mediator.Send(new DeleteRole.DeleteRoleCommand { RoleName = roleName }, cancellationToken);
                return Ok("Role deleted successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }
    }
}