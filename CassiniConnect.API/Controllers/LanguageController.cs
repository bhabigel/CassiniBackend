using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Application.Models.LanguageManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CassiniConnect.API.Controllers
{
    public class LanguageController : BaseController
    {
        [Authorize(Roles = "Admin")]
        [HttpPost("add")]
        public async Task<IActionResult> AddLanguage(string languageCode, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(languageCode))
            {
                return BadRequest("One or more of the obligatory fields are empty!");
            }

            try
            {
                await Mediator.Send(new AddLanguage.AddLanguageCommand { Code = languageCode }, cancellationToken);
                return Ok("Language added successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteLanguage(string languageCode, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(languageCode))
            {
                return BadRequest("One or more of the obligatory fields are empty!");
            }

            try
            {
                await Mediator.Send(new DeleteLanguage.DeleteLanguageCommand { Code = languageCode }, cancellationToken);
                return Ok("Language deleted successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("list")]
        public async Task<ActionResult<List<string>>> ListLanguages(CancellationToken cancellationToken)
        {
            try
            {
                var languages = await Mediator.Send(new ListLanguages.ListLanguagesRequest { }, cancellationToken);
                return Ok(languages);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }
    }
}