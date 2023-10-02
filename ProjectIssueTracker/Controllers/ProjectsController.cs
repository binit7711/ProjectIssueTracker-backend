using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectIssueTracker.Data;
using ProjectIssueTracker.Dtos;
using ProjectIssueTracker.Mappings;
using ProjectIssueTracker.Models;

namespace ProjectIssueTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly ApiDBContext _context;
        private readonly IMapper _mapper;

        public ProjectsController(ApiDBContext context,IMapper mapper)
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
                OwnerId= project.OwnerId,
            };

            _context.Projects.Add(newProject);
            _context.SaveChanges();

            return Ok(newProject);
        }

        [HttpGet("user/{id}")]
        public IActionResult GetAllProjectForUser([FromRoute] int id)
        {
            var projects = _context.Projects
                .Include(project=>project.Issues)
                .Include(project=>project.Owner)
                .Where(project => project.OwnerId == id);

            var projectDto =  _mapper.Map<List<ProjectDto>>(projects);

            return Ok(projectDto);
        }

        [HttpDelete("user/{id}")]
        public IActionResult DeleteProjectForuser([FromRoute] int id)
        {

        }

        [HttpPost("add-collaborators")]
        public IActionResult AddCollaborator([FromBody] AddCollaboratorDto request)
        {
            var project = _context.Projects
                .Include(i=>i.Collaborators)
                .FirstOrDefault(project => project.Id == request.ProjectId);
            if (project == null)
            {
                return NotFound();
            }
            var user = _context.Users.FirstOrDefault(u => u.Id == request.UserId);
            if (user == null)
            {
                return BadRequest();
            }

            if(project.Collaborators.Any(c=>c.UserId == request.UserId))
            {
                return BadRequest("User is already a collaborator on the project");
            }
            project.Collaborators.Add(new ProjectCollaborator { ProjectId=request.ProjectId,UserId = user.Id});

            _context.SaveChanges();
            return Ok();

        }
    }

}
