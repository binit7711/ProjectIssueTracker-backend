using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ProjectIssueTracker.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        [JsonIgnore]
        public string Password { get; set; } = string.Empty;
        public virtual  List<Project> OwnedProjects { get; set; }
        public virtual  List<ProjectCollaborator> CollaborativeProjects { get; set; }
        public virtual List<Comment> Comments { get; set; }
        public virtual List<Issue> CreatedIssues { get; set; }
    }
}
