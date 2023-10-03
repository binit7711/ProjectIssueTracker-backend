using System.ComponentModel.DataAnnotations;

namespace ProjectIssueTracker.Dtos.RequestDtos
{
    public class IssueCreateDto
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
