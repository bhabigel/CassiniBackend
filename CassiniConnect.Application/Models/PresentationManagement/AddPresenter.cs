using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Core.Models.Presentation;
using CassiniConnect.Core.Persistance;
using CassiniConnect.Core.Utilities.DTO;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CassiniConnect.Application.Models.PresentationManagement
{
    public class AddPresenter
    {
        public class AddPresenterCommand : IRequest<Unit>
        {
            public Guid UserId { get; set; }
            public string Image { get; set; } = string.Empty;
            public List<PresenterDetailDTO> Details { get; set; } = [];
        }

        public class AddPresenterCommandHandler : IRequestHandler<AddPresenterCommand, Unit>
        {
            private readonly DataContext dataContext;
            public AddPresenterCommandHandler(DataContext dataContext)
            {
                this.dataContext = dataContext;
            }

            public async Task<Unit> Handle(AddPresenterCommand command, CancellationToken cancellationToken)
            {
                if (string.IsNullOrEmpty(command.Image))
                {
                    throw new Exception("One or more of the obligatory fields are null or empty!");
                }
                var presenterExists = await dataContext.Presenters.AnyAsync(p => p.UserId == command.UserId);
                if (presenterExists)
                {
                    throw new Exception("Presenter associated with given user already exists!");
                }

                foreach (var desc in command.Details)
                {

                    var languageExists = await dataContext.LanguageCodes.AnyAsync(l => l.Id == desc.LanguageId, cancellationToken);
                    if (!languageExists)
                    {
                        throw new Exception("Language not found with given id!");
                    }
                }

                var presenter = new Presenter
                {
                    Id = Guid.NewGuid(),
                    UserId = command.UserId,
                    Image = command.Image
                };
                await dataContext.Presenters.AddAsync(presenter, cancellationToken);

                var details = new List<PresenterDetail>();
                foreach (var det in command.Details)
                {
                    var detail = new PresenterDetail
                    {
                        Id = Guid.NewGuid(),
                        LanguageId = det.LanguageId,
                        Description = det.Description,
                        Themes = det.Themes
                    };

                    details.Add(detail);
                    await dataContext.PresenterDetails.AddAsync(detail, cancellationToken);
                }
                
                presenter.Details = details;
                await dataContext.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}