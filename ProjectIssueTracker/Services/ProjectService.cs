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
        private object _mapper;

        public ProjectService(ApiDBContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<Project> CreateProject(ProjectCreateDto project)
        {
            var newProject = (new Project
            {
                Name = project.Name,
                Description = project.Description,
                OwnerId = project.OwnerId,
            });
            _dbContext.Projects.Add(newProject);
            await _dbContext.SaveChangesAsync();

            return newProject;
        }


        public async Task<List<Project>> GetOwnedProjectsForUserAsync(int userId)
        {

            var result = await _dbContext.Projects.Include(p => p.Owner).Where(p => p.OwnerId == userId).ToListAsync();

            return result;
        }

        public async Task DeleteProject(Project project)
        {
            _dbContext.Projects.Remove(project);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Project?> GetProject(int projectId, bool includeCollaborators = false, bool includeIssues = false)
        {
            var projects = _dbContext.Projects;

            if (includeCollaborators)
            {
                projects.Include(p => p.Collaborators);
            }

            if (includeIssues)
            {
                projects.Include(p => p.Issues);
            }

            return await projects.FirstOrDefaultAsync(project => project.Id == projectId);
        }

        public async Task<Project> AddCollaborator(Project project, int userId)
        {
            project.Collaborators.Add(new ProjectCollaborator { ProjectId = project.Id, UserId = userId });
            await _dbContext.SaveChangesAsync();
            return project;
        }

        public async Task<Project> UpdateProject(ProjectUpdateDto updateProject, Project project)
        {
            var result = await _dbContext.Projects.FindAsync(project);
            result.Description = updateProject.Description;
            result.Name = updateProject.Name;
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
