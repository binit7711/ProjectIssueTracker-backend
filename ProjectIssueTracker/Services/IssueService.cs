using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectIssueTracker.Data;
using ProjectIssueTracker.Dtos;

namespace ProjectIssueTracker.Services
{
    public class IssueService : IIssueService
    {
        private readonly IMapper _mapper;
        private readonly ApiDBContext _context;
        public IssueService(IMapper mapper, ApiDBContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<IssueDto> DeleteIssue(int issueId)
        {
            var issue = await _context.Issues
                .Include(i => i.Creator)
                .FirstOrDefaultAsync(i => i.Id == issueId);

            _context.Issues.Remove(issue);

            await _context.SaveChangesAsync();
            return _mapper.Map<IssueDto>(issue);
        }
    }
}
