using System.ComponentModel.DataAnnotations;

namespace ProjectIssueTracker.Dtos
{
    public class AddCollaboratorDto
    {
        [Required]
        public int ProjectId { get; set; }
        [Required]
        public int UserId { get; set; } 
    }
}
