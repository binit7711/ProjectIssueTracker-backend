using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectIssueTracker.Data;
using ProjectIssueTracker.Dtos;
using ProjectIssueTracker.Dtos.RequestDtos;
using ProjectIssueTracker.Extensions;
using ProjectIssueTracker.Models;
using ProjectIssueTracker.Services;

namespace ProjectIssueTracker.Controllers
{
    [Route("api/projects")]
    [ApiController]
    public class IssuesController : ControllerBase
    {
        private readonly ApiDBContext _context;
        private readonly IProjectService _projectService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IIssueService _issueService;

        public IssuesController(ApiDBContext context, IProjectService projectService, IUserService userService, IMapper mapper, IIssueService issueService)
        {
            _context = context;
            _projectService = projectService;
            _userService = userService;
            _mapper = mapper;
            _issueService = issueService;
        }

        [HttpPost("{projectId}/issues")]
        [Authorize]
        public async Task<IActionResult> CreateIssueForProjectAsync([FromBody] IssueCreateDto issue, [FromRoute] int projectId)
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault((c) => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);

            var project = await _projectService.GetProjectByIdAsync(projectId, includeCollaborators: false, includeIssues: true);

            var user = await _userService.GetUserById(userId);

            if(user == null)
            {
                return Unauthorized();
            }

            if (project == null)
            {
                return NotFound();
            }

            _context.Issues.Add(new Issue { Title = issue.Title, Description = issue.Description, CreatorId = user.Id, Status = issue.Status, ProjectId = projectId });

            _context.SaveChanges();

            var response = _mapper.Map<ProjectDto>(project);

            return Ok(response);

        }

        [HttpGet("{projectId}/issues")]
        [Authorize]
        public async Task<IActionResult> GetIssuesForProjectAsync([FromRoute] int projectId, int pageSize, int pageNumber)
        {
            var project = await _projectService.GetProjectByIdAsync(projectId, includeCollaborators: false, includeIssues: true);

            if (project == null)
            {
                return NotFound("Project doesn't exist");
            }

            var result = _context.Issues
                    .Where(i => i.ProjectId == projectId)
                    .Include(i => i.Creator)
                    .AsQueryable()
                    .Paginate(pageNumber, pageSize)
                    .Items.ToList();

            return Ok(_mapper.Map<List<IssueDto>>(result));
        }

        [HttpGet("{projectId}/issues/count")]
        [Authorize]
        public async Task<IActionResult> GetCountOfIssues([FromRoute] int projectId)
        {
            var count = await _context.Issues.Where(issue => issue.ProjectId == projectId).CountAsync();

            return Ok(new { count });
        }

        [HttpDelete("{projectId}/issues/{issueId}")]
        [Authorize]
        public async Task<IActionResult> DeleteIssueById([FromRoute] int projectId, [FromRoute] int issueId)
        {

           var issue =  await _issueService.DeleteIssue(issueId);

           return Ok(_mapper.Map<IssueDto>(issue));
        }

        [HttpPut("{projectId}/issues/{issueId}")]
        [Authorize]
        public async Task<IActionResult> UpdateIssueById([FromRoute] int issueId, [FromBody] IssueCreateDto updatedIssue)
        {
            var issue = await _context.Issues.FirstOrDefaultAsync(i => i.Id == issueId);

            if (issue == null)
            {
                return NotFound("Issue doesn't exist");
            }

            issue.Title = updatedIssue.Title;
            issue.Status = updatedIssue.Status;
            issue.Description = updatedIssue.Description;

            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<IssueDto>(issue));
        }

        [HttpGet("issues")]
        [Authorize]
        public Task<IActionResult> GetIssuesForUser(int pageSize, int pageNumber)
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault((c) => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Task.FromResult<IActionResult>(Unauthorized());
            }

            var userId = int.Parse(userIdClaim.Value);

            var issues = _context.Issues
                .Include(i => i.Project)
                .Include(i => i.Creator)
                .Where(i => i.CreatorId == userId)
                .Paginate(pageNumber, pageSize);

            return Task.FromResult<IActionResult>(Ok(new { count = issues.TotalCount, issues = _mapper.Map<List<IssueDto>>(issues.Items.ToList()) }));
        }
    }
}
