using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CassiniConnect.Core.Models.Teaching;

namespace CassiniConnect.Application.Mapping
{
    public class SubjectNameMappingProfile:Profile
    {
        public SubjectNameMappingProfile()
        {
            CreateMap<SubjectName, SubjectName>();
        }
    }
}