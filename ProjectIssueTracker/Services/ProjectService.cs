using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectIssueTracker.Data;
using ProjectIssueTracker.Dtos;
using ProjectIssueTracker.Models;

namespace ProjectIssueTracker.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ApiDBContext _dbContext;
        private readonly IMapper _mapper;

        public ProjectService(ApiDBContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<Project> CreateProject(ProjectCreateDto project)
        {
            var newProject = _mapper.Map<Project>(project);
            //var newProject = (new Project
            //{
            //    Name = project.Name,
            //    Description = project.Description,
            //    OwnerId = project.OwnerId,
            //});
            _dbContext.Projects.Add(newProject);
            await _dbContext.SaveChangesAsync();

            return newProject;
        }


        public async Task<IEnumerable<Project>> GetOwnedProjectsForUserAsync(int userId, int pageNumber = 1, int pageSize = 9)
        {

            var totalCount = await _dbContext.Projects
                 .Where(p => p.OwnerId == userId)
                 .CountAsync();

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var result = await _dbContext.Projects
                .Include(p => p.Owner)
                .Where(p => p.OwnerId == userId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return result;
        }

        public async Task DeleteProject(Project project)
        {
            var pro = await _dbContext.Projects
                .Include(p => p.Issues)
                .Include(p => p.Collaborators)
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.Id == project.Id);

            if (pro == null)
            {
                return;
            }

            foreach (var issue in project.Issues)
            {
                _dbContext.Issues.Remove(issue);
            }

            foreach (var collaborators in project.Collaborators)
            {
                _dbContext.ProjectCollaborators.Remove(collaborators);
            }

            _dbContext.Projects.Remove(pro);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Project?> GetProject(int projectId, bool includeCollaborators, bool includeIssues)
        {
            var projects = _dbContext.Projects.AsQueryable<Project>();

            if (includeCollaborators)
            {
                projects = projects.Include(p => p.Collaborators).ThenInclude(p => p.User);
            }

            if (includeIssues)
            {
                projects = projects.Include(p => p.Issues);
            }

            return await projects.Include(p => p.Owner)
                .FirstOrDefaultAsync(project => project.Id == projectId);
        }

        public async Task<Project?> AddCollaborator(Project project, int userId)
        {
            //var collaborator = _dbContext.ProjectCollaborators.Add(new ProjectCollaborator { ProjectId = project.Id, UserId = userId });

            //await _dbContext.SaveChangesAsync();
            //var proj = await _dbContext.Projects
            //    .Include(p => p.Collaborators)
            //    .FirstOrDefaultAsync(p => p.Id == project.Id);

            //if (proj == null)
            //{
            //    return null;
            //}

            _dbContext.ProjectCollaborators.Add(new ProjectCollaborator { ProjectId = project.Id, UserId = userId });

            await _dbContext.SaveChangesAsync();
            var p = await _dbContext.Projects
                .Include(p => p.Collaborators)
                .FirstOrDefaultAsync(p => p.Id == project.Id);
            return p;
        }

        public async Task<Project> UpdateProject(ProjectUpdateDto updateProject, Project project)
        {
            var result = await _dbContext.Projects.FindAsync(project.Id);

            _mapper.Map(updateProject, project);
            await _dbContext.SaveChangesAsync();
            return result;
        }

        public Task DeleteCollaborator(ProjectCollaborator projectCollaborator)
        {
            _dbContext.ProjectCollaborators.Remove(projectCollaborator);
            return Task.CompletedTask;
        }

        public async Task<ProjectCollaborator> GetCollaborator(int userId, int projectId)
        {
            var collaborator = await _dbContext.ProjectCollaborators
                .FirstOrDefaultAsync(project => project.ProjectId == projectId && project.UserId == userId);

            return collaborator;
        }
    }
}
