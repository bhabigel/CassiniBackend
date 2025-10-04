using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Core.Models.EventCalendar;
using CassiniConnect.Core.Persistance;
using CassiniConnect.Core.Utilities.DTO;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CassiniConnect.Application.Models.EventManagement
{
    public class AddEvent
    {
        public class AddEventCommand : IRequest<Unit>
        {
            public Guid CreatorId { get; set; }
            public DateTime Date { get; set; }
            public List<EventDetailDTO> EventDetails { get; set; } = [];
        }

        public class AddEventCommandHandler : IRequestHandler<AddEventCommand, Unit>
        {
            private readonly DataContext dataContext;
            public AddEventCommandHandler(DataContext dataContext)
            {
                this.dataContext = dataContext;
            }

            public async Task<Unit> Handle(AddEventCommand command, CancellationToken cancellationToken)
            {
                var newEvent = new Event
                {
                    Id = Guid.NewGuid(),
                    CreatorId = command.CreatorId,
                    Date = command.Date,
                };

                await dataContext.Events.AddAsync(newEvent, cancellationToken);

                var eventDetails = new List<EventDetail>();
                foreach (var detail in command.EventDetails)
                {
                    var languageExists = await dataContext.LanguageCodes.AnyAsync(l => l.Id == detail.LanguageId);
                    if (!languageExists)
                    {
                        throw new Exception("Language not found with given id!");
                    }

                    var eventDetail = new EventDetail
                    {
                        Id = Guid.NewGuid(),
                        EventId = newEvent.Id,
                        LanguageId = detail.LanguageId,
                        Title = detail.Title,
                        Description = detail.Description,
                        Location = detail.Location
                    };

                    await dataContext.EventDetails.AddAsync(eventDetail, cancellationToken);
                }
                await dataContext.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}