using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Core.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CassiniConnect.Application.Models.LanguageManagement
{
    public class ListLanguages
    {
        public class ListLanguagesRequest : IRequest<List<string>>
        {
        }

        public class ListLanguagesRequestHandler: IRequestHandler<ListLanguagesRequest, List<string>>
        {
            private readonly DataContext dataContext;
            public ListLanguagesRequestHandler(DataContext dataContext)
            {
                this.dataContext = dataContext;
            }

            public async Task<List<string>> Handle(ListLanguagesRequest request, CancellationToken cancellationToken)
            {
                var languageCodes = await dataContext.LanguageCodes.ToListAsync(cancellationToken);
                if(languageCodes == null)
                {
                    throw new Exception("No languagecode was found!");
                }

                var languages = languageCodes.Select(l => l.Code).ToList();
                return languages;
            }
        }
    }
}