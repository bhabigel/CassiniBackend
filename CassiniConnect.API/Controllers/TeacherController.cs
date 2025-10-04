using System.ComponentModel.DataAnnotations;
using CassiniConnect.Application.Models.TeacherManagement.Subjects;
using CassiniConnect.Application.Models.TeacherManagement.Teachers;
using CassiniConnect.Application.Models.UserManagement;
using CassiniConnect.Core.Models.Teaching;
using CassiniConnect.Core.Utilities.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CassiniConnect.API.Controllers
{
    public class TeacherController : BaseController
    {
        [Authorize(Policy = "Admin")]
        [HttpPost("add")]
        public async Task<IActionResult> AddTeacher(IFormFile profilePicture, [FromForm] string userEmail, [FromForm] float rate, [FromForm] List<TeacherDescriptionDTO> descriptions, [FromForm] List<string> subjects, CancellationToken cancellationToken)
        {
            Console.WriteLine($"desc {descriptions[0].Description}");
            if (profilePicture == null || profilePicture.Length == 0 || string.IsNullOrEmpty(userEmail) || descriptions == null || subjects == null)
            {
                return BadRequest("Some or more of the obligatory fields are missing!");
            }

            List<string> allowedMimeTypes = ["image/png", "image/jpeg"];
            if (!allowedMimeTypes.Contains(profilePicture.ContentType))
            {
                return BadRequest("Profile picture should be of type png or jpeg!");
            }

            try
            {
                var userId = await Mediator.Send(new GetUserId.GetUserIdRequest { Email = userEmail }, cancellationToken);
                var fileId = Guid.NewGuid();
                var extension = Path.GetExtension(profilePicture.FileName);
                var fileName = $"{fileId}{extension}";

                using var memoryStream = new MemoryStream();
                await profilePicture.CopyToAsync(memoryStream);
                byte[] fileData = memoryStream.ToArray();
                await FileService.SaveFileAsync(fileData, fileName, cancellationToken);

                await Mediator.Send(new AddTeacher.AddTeacherCommand { UserId = userId, Image = fileName, Rate = rate, Descriptions = descriptions, Subjects = subjects }, cancellationToken);
                return Ok("New teacher was successfully added!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [Authorize(Policy = "Admin")]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteTeacher(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                await Mediator.Send(new DeleteTeacher.DeleteTeacherCommand { Id = id }, cancellationToken);
                return Ok("Teacher deleted successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("list")]
        public async Task<ActionResult<List<TeacherDTO>>> GetTeachers(string languageCode, CancellationToken cancellationToken)
        {
            try
            {
                var teachers = await Mediator.Send(new ListTeachers.ListTeacherCommand { LanguageCode = languageCode }, cancellationToken);
                return Ok(teachers);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("filter")]
        public async Task<ActionResult<List<TeacherDTO>>> FilterTeachers(string nameSearch, string languageCode, CancellationToken cancellationToken)
        {
            try
            {
                var teachers = await Mediator.Send(new FilterTeachers.FilterTeachersRequest {NameSearch = nameSearch, LanguageCode = languageCode }, cancellationToken);
                return Ok(teachers);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [Authorize(Policy = "Admin")]
        [HttpPost("add-subject")]
        public async Task<IActionResult> AddSubjectToTeacher(string email, string subjectName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(subjectName))
            {
                return BadRequest("Some or more of the obligatory fields are empty!");
            }

            var emailAttribute = new EmailAddressAttribute();
            if (!emailAttribute.IsValid(email))
            {
                return BadRequest("Invalid email format!");
            }

            try
            {
                var userId = await Mediator.Send(new GetUserId.GetUserIdRequest { Email = email }, cancellationToken);
                var subject = await Mediator.Send(new GetSubject.GetSubjectRequest { Code = subjectName }, cancellationToken);
                var teacherId = await Mediator.Send(new GetTeacherId.GetTeacherIdRequest { UserId = userId }, cancellationToken);

                await Mediator.Send(new AddSubjectToTeacher.AddSubjectToTeacherCommand { SubjectId = subject.Id, TeacherId = teacherId }, cancellationToken);
                return Ok("Subject added to teacher successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }
    }
}