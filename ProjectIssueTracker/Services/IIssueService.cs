using ProjectIssueTracker.Dtos;

namespace ProjectIssueTracker.Services
{
    public interface IIssueService
    {
        Task<IssueDto> DeleteIssue(int issueId);
    }
}
