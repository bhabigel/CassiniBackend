using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Core.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CassiniConnect.Application.Models.LanguageManagement
{
    public class DeleteLanguage
    {
        public class DeleteLanguageCommand : IRequest<Unit>
        {
            public string Code { get; set; } = string.Empty;
        }

        public class DeleteLanguageCommandHandler : IRequestHandler<DeleteLanguageCommand, Unit>
        {
            private readonly DataContext dataContext;
            public DeleteLanguageCommandHandler(DataContext dataContext)
            {
                this.dataContext = dataContext;
            }

            public async Task<Unit> Handle(DeleteLanguageCommand command, CancellationToken cancellationToken)
            {
                if(string.IsNullOrEmpty(command.Code))
                {
                    throw new Exception("One or more of the obligatory fields are empty!");
                }

                var language = await dataContext.LanguageCodes.FirstOrDefaultAsync(l => l.Code == command.Code);
                if(language == null)
                {
                    throw new Exception("Language with given code doesn't exist!");
                }

                dataContext.LanguageCodes.Remove(language);
                await dataContext.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}