using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Core.Models.Helpers;
using CassiniConnect.Core.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CassiniConnect.Application.Models.LanguageManagement
{
    public class AddLanguage
    {
        public class AddLanguageCommand : IRequest<Unit>
        {
            public string Code { get; set; } = string.Empty;
        }

        public class AddLanguageCommandHandler : IRequestHandler<AddLanguageCommand, Unit>
        {
            private readonly DataContext dataContext;
            public AddLanguageCommandHandler(DataContext dataContext)
            {
                this.dataContext = dataContext;
            }

            public async Task<Unit> Handle(AddLanguageCommand command, CancellationToken cancellationToken)
            {
                if (string.IsNullOrEmpty(command.Code))
                {
                    throw new Exception("One or more of the obligatory fields are null!");
                }

                var languageExists = await dataContext.LanguageCodes.AnyAsync(l => l.Code == command.Code);
                if (languageExists)
                {
                    throw new Exception("Language with given code already exists!");
                }

                var language = new LanguageCode
                {
                    Id = Guid.NewGuid(),
                    Code = command.Code
                };
                await dataContext.LanguageCodes.AddAsync(language);
                return Unit.Value;
            }
        }
    }
}