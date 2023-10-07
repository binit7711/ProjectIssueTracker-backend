using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectIssueTracker.Data;
using ProjectIssueTracker.Dtos;
using ProjectIssueTracker.Dtos.RequestDtos;
using ProjectIssueTracker.Models;
using ProjectIssueTracker.Services;
using System.Drawing.Printing;

namespace ProjectIssueTracker.Controllers
{
    [Route("api/projects")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly ApiDBContext _context;
        private readonly IMapper _mapper;
        private readonly IProjectService _projectService;
        private readonly IUserService _userService;

        public ProjectsController(ApiDBContext context, IMapper mapper, IProjectService projectService, IUserService userService)
        {
            _context = context;
            _mapper = mapper;
            _projectService = projectService;
            _userService = userService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateProjectForUserAsync([FromBody] ProjectCreateDto project)
        {
            var user = await _userService.GetUserById(project.OwnerId);

            if (user == null)
            {
                return NotFound("User doesn't exist");
            }

            var newProject = await _projectService.CreateProject(project);

            return Ok(_mapper.Map<ProjectDto>(newProject));
        }

        [HttpGet("{projectId}")]
        [Authorize]
        public async Task<IActionResult> GetProjectAsync([FromRoute] int projectId)
        {
            var project = await _projectService.GetProject(projectId, true, true);

            if (project == null)
            {
                return NotFound("Project not found");
            }

            return Ok(_mapper.Map<ProjectDto>(project));
        }

        [HttpPut("{projectId}")]
        [Authorize(Policy = "ProjectOwnerPolicy")]
        public async Task<IActionResult> UpdateProject([FromBody] ProjectUpdateDto projectUpdate, [FromRoute] int projectId)
        {
            var oldProject = await _projectService.GetProject(projectId, false, false);

            if (oldProject == null)
            {
                return NotFound("Project not found");
            }

            var newProject = await _projectService.UpdateProject(projectUpdate, oldProject);

            return Ok(_mapper.Map<ProjectDto>(newProject));

        }

        [HttpGet("user/{id}")]
        [Authorize]
        public async Task<IActionResult> GetAllProjectForUserAsync([FromRoute] int id, int pageNumber = 1, int pageSize = 9)
        {

            var projects = await _projectService.GetOwnedProjectsForUserAsync(id, pageNumber, pageSize);

            var projectDto = _mapper.Map<List<ProjectDto>>(projects);

            return Ok(projectDto);
        }

        [HttpGet("user/{id}/count")]
        [Authorize]
        public async Task<IActionResult> GetPageCount([FromRoute] int id)
        {
            var totalCount = await _context.Projects
                .Where(p => p.OwnerId == id)
                .CountAsync();


            return Ok(new { count = totalCount });
        }

        [HttpDelete("{projectId}")]
        [Authorize(Policy = "ProjectOwnerPolicy")]
        public async Task<IActionResult> DeleteProjectForuser([FromRoute] int projectId)
        {

            var project = await _projectService.GetProject(projectId, false, false);

            if (project == null)
            {
                return NotFound();
            }

            await _projectService.DeleteProject(project);

            return Ok(_mapper.Map<ProjectDto>(project));
        }

        [HttpPost("add-collaborators/{projectId}")]
        [Authorize(Policy = "ProjectOwnerPolicy")]
        public async Task<IActionResult> AddCollaboratorAsync([FromBody] AddCollaboratorDto request, [FromRoute] int projectId)
        {

            var project = await _projectService.GetProject(projectId, true, false);

            if (project == null)
            {
                return NotFound();
            }
            if (request.UserId == 0)
            {
                return BadRequest();
            }

            var user = await _userService.GetUserById(request.UserId);

            if (user == null)
            {
                return BadRequest();
            }

            if (project.OwnerId == request.UserId)
            {
                return BadRequest("User is already owner of the project");
            }

            if (project.Collaborators != null && project.Collaborators.Any(c => c.UserId == request.UserId))
            {
                return BadRequest("User is already a collaborator on the project");
            }

            var pro = await _projectService.AddCollaborator(project, user.Id);

            return Ok(_mapper.Map<ProjectDto>(pro));

        }

        [HttpDelete("remove-collaborators/{projectId}")]
        [Authorize(Policy = "ProjectOwnerPolicy")]
        public async Task<IActionResult> RemoveCollaborator([FromBody] RemoveCollaboratorDto request, [FromRoute] int projectId)
        {
            var projectCollaborator = await _projectService.GetCollaborator(request.CollaboratorId, projectId);

            if (projectCollaborator == null)
            {
                return NotFound();
            }

            await _projectService.DeleteCollaborator(projectCollaborator);

            return Ok();
        }

        [HttpPost("{projectId}/issues")]
        [Authorize]
        public async Task<IActionResult> CreateIssueForProjectAsync([FromBody] IssueCreateDto issue, [FromRoute] int projectId)
        {
            var project = await _projectService.GetProject(projectId, includeCollaborators: false, includeIssues: true);

            if (project == null)
            {
                return NotFound();
            }

            _context.Issues.Add(new Issue { Title = issue.Title, Description = issue.Description, CreatorId = project.Owner.Id, Status = issue.Status, ProjectId = projectId });

            _context.SaveChanges();

            var response = _mapper.Map<ProjectDto>(project);

            return Ok(response);

        }

        [HttpGet("{projectId}/issues")]
        public async Task<IActionResult> GetIssuesForProjectAsync([FromRoute] int projectId, int pageSize, int pageNumber)
        {
            var project = await _projectService.GetProject(projectId, includeCollaborators: false, includeIssues: true);

            if (project == null)
            {
                return NotFound("Project doesn't exist");
            }

            //var issues = await _context.Issues.Where(issue => issue.ProjectId == projectId)
            //    .ToListAsync();

            var issues = await _context.Issues
                    .Where(i => i.ProjectId == projectId)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

            return Ok(_mapper.Map<List<IssueDto>>(issues));
        }

        [HttpGet("{projectId}/issues/count")]
        public async Task<IActionResult> GetCountOfIssues([FromRoute] int projectId)
        {
            var count = await _context.Issues.Where(issue => issue.ProjectId == projectId).CountAsync();

            return Ok(new { count });
        }

        [HttpDelete("{projectId}/issues/{issueId}")]
        [Authorize]
        public async Task<IActionResult> DeleteIssueById([FromRoute] int projectId, [FromRoute] int issueId)
        {
            var issue = await _context.Issues.Include(i => i.CreatorId).FirstOrDefaultAsync(i => i.Id == issueId);

            if (issue == null)
            {
                return NotFound("Issue doesn't exist");
            }

            _context.Issues.Remove(issue);

            await _context.SaveChangesAsync();

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
    }
}
