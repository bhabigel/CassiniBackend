using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Core.Models.EventCalendar;
using CassiniConnect.Core.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CassiniConnect.Application.Models.EventManagement
{
    public class ListEvents
    {
        public class ListEventsRequest : IRequest<List<Event>>
        {
            public string LanguageCode { get; set; } = string.Empty;
        }

        public class ListEventsHandler : IRequestHandler<ListEventsRequest, List<Event>>
        {
            private readonly DataContext dataContext;
            public ListEventsHandler(DataContext dataContext)
            {
                this.dataContext = dataContext;
            }

            public async Task<List<Event>> Handle(ListEventsRequest request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrEmpty(request.LanguageCode))
                {
                    throw new Exception("One or more of the obligatory fields are empty!");
                }

                var language = await dataContext.LanguageCodes.FirstOrDefaultAsync(x => x.Code == request.LanguageCode, cancellationToken);
                if (language == null)
                {
                    throw new Exception("No language found by given code!");
                }
                var events = await dataContext.Events.Include(e => e.EventDetails)
                                                     .Select(e => new Event {
                                                        Id = e.Id,
                                                        CreatorId = e.CreatorId,
                                                        Date = e.Date,
                                                        EventDetails = e.EventDetails.Where(d => d.LanguageId == language.Id).ToList(),
                                                     }).
                                                     ToListAsync(cancellationToken);
                if (events == null)
                {
                    throw new Exception("No events were found!");
                }

                return events;
            }
        }
    }
}