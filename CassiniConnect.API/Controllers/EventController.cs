using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Application.Models.EventManagement;
using CassiniConnect.Core.Models.EventCalendar;
using CassiniConnect.Core.Models.Helpers;
using CassiniConnect.Core.Utilities.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CassiniConnect.API.Controllers
{
    public class EventController : BaseController
    {
        [Authorize(Roles = "Admin")]
        [HttpPost("add")]
        public async Task<IActionResult> AddEvent(Guid creatorId, DateTime date, List<EventDetailDTO> eventDetails, CancellationToken cancellationToken)
        {
            try
            {
                await Mediator.Send(new AddEvent.AddEventCommand { CreatorId = creatorId, Date = date, EventDetails = eventDetails });
                return Ok("Event added successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteEvent(Guid eventId, CancellationToken cancellationToken)
        {
            try
            {
                await Mediator.Send(new DeleteEvent.DeleteEventCommand { EventId = eventId });
                return Ok("Event deleted successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("list")]
        public async Task<ActionResult<List<Event>>> ListEvents(string languageCode, CancellationToken cancellationToken)
        {
            try
            {
                var events = await Mediator.Send(new ListEvents.ListEventsRequest { LanguageCode = languageCode }, cancellationToken);
                return Ok(events);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }
    }
}