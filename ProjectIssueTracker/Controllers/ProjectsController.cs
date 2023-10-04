using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectIssueTracker.Data;
using ProjectIssueTracker.Dtos;
using ProjectIssueTracker.Dtos.RequestDtos;
using ProjectIssueTracker.Models;
using ProjectIssueTracker.Services;

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
        public IActionResult CreateProjectForUser([FromBody] ProjectCreateDto project)
        {
            var user = _userService.GetUserById(project.OwnerId);

            if (user == null)
            {
                return NotFound("User doesn't exist");
            }

            var newProject = _projectService.CreateProject(project);

            return Ok(_mapper.Map<ProjectDto>(newProject));
        }

        [HttpGet("projectId")]
        [Authorize]
        public async Task<IActionResult> GetProjectAsync([FromRoute] int projectId)
        {
            var project = await _projectService.GetProject(projectId);

            if (project == null)
            {
                return NotFound("Project not found");
            }

            return Ok(_mapper.Map<ProjectDto>(project));
        }

        [HttpPut("projectId")]
        [Authorize(Policy ="ProjectOwnerPolicy")]
        public async Task<IActionResult> UpdateProject([FromBody] ProjectUpdateDto projectUpdate, [FromRoute] int projectId)
        {
            var oldProject = await _projectService.GetProject(projectId);

            if (oldProject == null)
            {
                return NotFound("Project not found");
            }

            var newProject = await _projectService.UpdateProject(projectUpdate, oldProject);

            return Ok(_mapper.Map<ProjectDto>(newProject)); 

        }

        [HttpGet("user/{id}")]
        [Authorize]
        public async Task<IActionResult> GetAllProjectForUserAsync([FromRoute] int id)
        {

            var projects = await _projectService.GetOwnedProjectsForUserAsync(id);

            var projectDto = _mapper.Map<List<ProjectDto>>(projects);

            return Ok(projectDto);
        }

        [HttpDelete("{projectId}")]
        [Authorize(Policy = "ProjectOwnerPolicy")]
        public async Task<IActionResult> DeleteProjectForuser([FromRoute] int projectId)
        {

            var project = await _projectService.GetProject(projectId);

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

            var project = await _projectService.GetProject(projectId, includeCollaborators: true);

            if (project == null)
            {
                return NotFound();
            }

            var user = _userService.GetUserById(request.UserId);

            if (user == null)
            {
                return BadRequest();
            }

            if (project.OwnerId == request.UserId)
            {
                return BadRequest("User is already owner of the project");
            }

            if (project.Collaborators.Any(c => c.UserId == request.UserId))
            {
                return BadRequest("User is already a collaborator on the project");
            }

            await _projectService.AddCollaborator(project, user.Id);

            return Ok(_mapper.Map<ProjectDto>(project));

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
        public IActionResult CreateIssueForProject([FromBody] IssueCreateDto issue, [FromRoute] int projectId)
        {
            var project = _context.Projects
                .Include(p => p.Issues)
                .Include(p => p.Owner)
                .FirstOrDefault(proj => proj.Id == projectId);

            if (project == null)
            {
                return NotFound();
            }

            project.Issues.Add(new Issue { Title = issue.Title, Description = issue.Description, CreatorId = project.Owner.Id });

            _context.SaveChanges();

            var response = _mapper.Map<ProjectDto>(project);
            return Ok(response);

        }

        //[HttpPut("{projectId}/issues")]
        //[HttpGet("{projectId}/issues")]
        //[HttpDelete("{projectId}/issues/{issueId}")]
    }
}
