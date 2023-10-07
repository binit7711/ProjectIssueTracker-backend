using Microsoft.AspNetCore.Mvc;
using ProjectIssueTracker.Dtos;
using ProjectIssueTracker.Models;

namespace ProjectIssueTracker.Services
{
    public interface IProjectService
    {
        Task<IEnumerable<Project>> GetOwnedProjectsForUserAsync(int userId, int pageNumber = 1, int pageSize = 9);

        Task<Project> CreateProject(ProjectCreateDto project);

        Task DeleteProject(Project project);

        Task<Project?> GetProject(int projectId, bool includeCollaborators, bool includeIssues);

        Task<Project?> AddCollaborator(Project project, int userId);

        Task<Project> UpdateProject(ProjectUpdateDto updateProject, Project project);

        Task DeleteCollaborator(ProjectCollaborator projectCollaborator);

        Task<ProjectCollaborator> GetCollaborator(int userId, int projectId);
    }
}
