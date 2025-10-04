using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Core.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CassiniConnect.Application.Models.EventManagement
{
    public class DeleteEvent
    {
        public class DeleteEventCommand : IRequest<Unit>
        {
            public Guid EventId { get; set; }
        }

        public class DeleteEventCommandHandler : IRequestHandler<DeleteEventCommand, Unit>
        {
            private readonly DataContext dataContext;
            public DeleteEventCommandHandler(DataContext dataContext)
            {
                this.dataContext = dataContext;
            }

            public async Task<Unit> Handle(DeleteEventCommand deleteEventCommand, CancellationToken cancellationToken)
            {
                var eventToDelete = await dataContext.Events.FirstOrDefaultAsync(e => e.Id == deleteEventCommand.EventId);
                if (eventToDelete == null)
                {
                    throw new Exception("Event not found by given id!");
                }

                dataContext.Events.Remove(eventToDelete);
                await dataContext.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}