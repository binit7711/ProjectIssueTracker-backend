using ProjectIssueTracker.Models;
using System.ComponentModel.DataAnnotations;

namespace ProjectIssueTracker.Dtos
{
    public class ProjectCreateDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public int OwnerId { get; set; }
    }
}
