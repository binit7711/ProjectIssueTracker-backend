using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
    public class ProjectsController : ControllerBase
    {
        private readonly ApiDBContext _context;
        private readonly IMapper _mapper;

        public ProjectsController(ApiDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        public IActionResult CreateProjectForUser([FromBody] ProjectCreateDto project)
        {
            var user = _context.Users.FirstOrDefault(user => user.Id == project.OwnerId);

            if (user == null)
            {
                return NotFound("User doesn't exist");
            }

            var newProject = new Project
            {
                Name = project.Name,
                OwnerId = project.OwnerId,
            };

            _context.Projects.Add(newProject);
            _context.SaveChanges();

            return Ok(newProject);
        }

        [HttpGet("user/{id}")]
        public IActionResult GetAllProjectForUser([FromRoute] int id)
        {
            var projects = _context.Projects
                .Include(project => project.Issues)
                .Include(project => project.Owner)
                .Where(project => project.OwnerId == id);

            var projectDto = _mapper.Map<List<ProjectDto>>(projects);

            return Ok(projectDto);
        }

        [HttpDelete("{projectId}")]
        [Authorize(Policy = "ProjectOwnerPolicy")]
        public IActionResult DeleteProjectForuser([FromRoute] int projectId)
        {

            var project = _context.Projects.FirstOrDefault(p => p.Id == projectId);
            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);

            _context.SaveChanges();

            return Ok(_mapper.Map<ProjectDto>(project));
        }

        [HttpPost("add-collaborators/{projectId}")]
        [Authorize(Policy = "ProjectOwnerPolicy")]
        public IActionResult AddCollaborator([FromBody] AddCollaboratorDto request, [FromRoute]int projectId)
        {
            var project = _context.Projects
                .Include(i => i.Collaborators)
                .FirstOrDefault(project => project.Id == projectId);

            if (project == null)
            {
                return NotFound();
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == request.UserId);

            if (user == null)
            {
                return BadRequest();
            }

            if(project.OwnerId == request.UserId)
            {
                return BadRequest("User is already owner of the project");
            }
            if (project.Collaborators.Any(c => c.UserId == request.UserId))
            {
                return BadRequest("User is already a collaborator on the project");
            }

            project.Collaborators.Add(new ProjectCollaborator { ProjectId = projectId, UserId = user.Id });

            _context.SaveChanges();

            return Ok(_mapper.Map<ProjectDto>(project));

        }

        [HttpDelete("remove-collaborators/{projectId}")]
        [Authorize(Policy ="ProjectOwnerPolicy")]
        public IActionResult RemoveCollaborator([FromBody] RemoveCollaboratorDto request, [FromRoute]int  projectId)
        {
            var projectCollaborator = _context.ProjectCollaborators.FirstOrDefault(p => p.ProjectId == projectId && p.UserId == request.CollaboratorId);

            if(projectCollaborator == null)
            {
                return NotFound();
            }

            _context.ProjectCollaborators.Remove(projectCollaborator);

            _context.SaveChanges();

            return Ok();
        }

    }
}
