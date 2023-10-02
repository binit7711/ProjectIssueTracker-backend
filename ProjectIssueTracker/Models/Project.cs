using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ProjectIssueTracker.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public int OwnerId { get; set; }
        [JsonIgnore]
        public virtual User Owner{ get; set; }
        public virtual List<Issue> Issues { get; set;}
        public virtual List<ProjectCollaborator> Collaborators { get; set;}
    }
}
