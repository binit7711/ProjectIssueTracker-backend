using Microsoft.AspNetCore.Mvc;
using ProjectIssueTracker.Dtos;
using ProjectIssueTracker.Models;

namespace ProjectIssueTracker.Services
{
    public interface IProjectService
    {
        Task<List<Project>> GetOwnedProjectsForUserAsync(int userId);

        Task<Project> CreateProject(ProjectCreateDto project);

        Task DeleteProject(Project project);

        Task<Project?> GetProject(int projectId, bool includeCollaborators = false, bool includeIssues = false);

        Task<Project> AddCollaborator(Project project, int userId);

        Task<Project> UpdateProject(ProjectUpdateDto updateProject,Project project);

        Task DeleteCollaborator(ProjectCollaborator projectCollaborator);

        Task<ProjectCollaborator> GetCollaborator(int userId, int projectId);
    }
}
