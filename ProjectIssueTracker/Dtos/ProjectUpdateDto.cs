using System.ComponentModel.DataAnnotations;

namespace ProjectIssueTracker.Dtos
{
    public class ProjectUpdateDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get;set; } = string.Empty;

    }
}
