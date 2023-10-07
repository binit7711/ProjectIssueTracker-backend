using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectIssueTracker.Data;
using ProjectIssueTracker.Dtos;
using ProjectIssueTracker.Dtos.RequestDtos;
using ProjectIssueTracker.Models;

namespace ProjectIssueTracker.Controllers
{
    [Route("api/projects")]
    [ApiController]
    public class IssuesController : ControllerBase
    {
        private readonly ApiDBContext _context;

        public IssuesController(ApiDBContext context)
        {
            _context = context;
        }

        //[Route("api/projects/{projectId}/issues")]
        //[Route("api/projects/{projectId}/issues/{issueId}")]
        //[Route("api/issues/user")]
        //[Route("api/issues/{issueId}")]

    }
}
