using CassiniConnect.Application.Models.TeacherManagement.Subjects;
using CassiniConnect.Core.Models.Helpers;
using CassiniConnect.Core.Models.Teaching;
using CassiniConnect.Core.Utilities.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CassiniConnect.API.Controllers
{
    public class SubjectController : BaseController
    {
        [Authorize(Policy = "Admin")]
        [HttpPost("add")]
        public async Task<IActionResult> AddSubject(string subjectCode, List<SubjectNameDTO> subjectNames, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(subjectCode))
            {
                return BadRequest("Subject name is empty!");
            }

            try
            {
                await Mediator.Send(new AddSubject.AddSubjectCommand { SubjectCode = subjectCode, SubjectNames = subjectNames }, cancellationToken);
                return Ok("Subject was created successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [Authorize(Policy = "Admin")]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteSubject(string subjectCode, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(subjectCode))
            {
                return BadRequest("Subject name is empty!");
            }

            try
            {
                await Mediator.Send(new DeleteSubject.DeleteSubjectCommand { Code = subjectCode }, cancellationToken);
                return Ok("Subject deleted successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("list")]
        public async Task<ActionResult<List<string>>> ListSubjects(string languageCode, CancellationToken cancellationToken)
        {
            try
            {
                var subjects = await Mediator.Send(new ListSubjects.ListSubjectsRequest { LanguageCode = languageCode }, cancellationToken);
                if (subjects == null)
                {
                    return BadRequest("No subject was found!");
                }
                return Ok(subjects);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [Authorize(Policy = "Admin")]
        [HttpGet("codes")]
        public async Task<ActionResult<List<SubjectCode>>> GetCodes(string languageCode, CancellationToken cancellationToken)
        {
            try
            {
                var codes = await Mediator.Send(new ListSubjectCodes.ListSubjectCodesRequest { LanguageCode = languageCode }, cancellationToken);
                return Ok(codes);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }
    }
}