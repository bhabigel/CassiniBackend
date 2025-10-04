using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using CassiniConnect.Application.Utilities;

namespace CassiniConnect.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class BaseController : ControllerBase
    {
        private IMediator mediator;
        private IFileService fileService;
        protected IMediator Mediator => mediator ??= HttpContext.RequestServices.GetService<IMediator>();
        protected IFileService FileService => fileService ??= HttpContext.RequestServices.GetService<IFileService>();
    }
}