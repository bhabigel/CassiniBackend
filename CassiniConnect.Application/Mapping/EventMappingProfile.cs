using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CassiniConnect.Core.Models.EventCalendar;

namespace CassiniConnect.Application.Mapping
{
    public class EventMappingProfile : Profile
    {
        public EventMappingProfile()
        {
            CreateMap<Event, Event>();
        }
    }
}