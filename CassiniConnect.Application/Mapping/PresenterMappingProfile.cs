using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CassiniConnect.Core.Models.Presentation;

namespace CassiniConnect.Application.Mapping
{
    public class PresenterMappingProfile:Profile
    {
        public PresenterMappingProfile()
        {
            CreateMap<Presenter, Presenter>();
        }
    }
}