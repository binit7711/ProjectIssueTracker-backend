using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectIssueTracker.Dtos;
using ProjectIssueTracker.Dtos.RequestDtos;
using ProjectIssueTracker.Services;

namespace ProjectIssueTracker.Controllers
{
    [Route("api/projects")]
    [ApiController]
    public class CollaboratorsController : ControllerBase
    {
        private readonly ICollaboratorService _collaboratorService;
        private readonly IProjectService _projectService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public CollaboratorsController(ICollaboratorService collaboratorService, IProjectService projectService, IUserService userService, IMapper mapper)
        {
            _collaboratorService = collaboratorService;
            _projectService = projectService;
            _userService = userService;
            _mapper = mapper;
        }


        [HttpPost("{projectId}/collaborators")]
        [Authorize(Policy = "ProjectOwnerPolicy")]
        public async Task<IActionResult> AddCollaboratorAsync([FromBody] AddCollaboratorDto request, [FromRoute] int projectId)
        {

            var project = await _projectService.GetProjectByIdAsync(projectId, true, false);

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

            await _collaboratorService.AddCollaboratorToProjectAsync(project, user.Id);

            return Ok();
        }

        [HttpDelete("{projectId}/collaborators/{collaboratorId}")]
        [Authorize(Policy = "ProjectOwnerPolicy")]
        public async Task<IActionResult> RemoveCollaborator([FromRoute] int collaboratorId, [FromRoute] int projectId)
        {
            var projectCollaborator = await _projectService.GetCollaborator(collaboratorId, projectId);

            if (projectCollaborator == null)
            {
                return NotFound();
            }

            await _collaboratorService.RemoveCollaboratorFromProjectAsync(projectCollaborator);

            return Ok();
        }

        [HttpGet("{projectId}/collaborators")]
        //[Authorize]
        public async Task<IActionResult> SearchCollaborators(int projectId, string searchQuery)
        {
            var collaborators = await _collaboratorService.SearchCollaboratorByNameOrEmail(projectId, searchQuery);

            return Ok(_mapper.Map<IEnumerable<UserDto>>(collaborators));
        }
    }
}
