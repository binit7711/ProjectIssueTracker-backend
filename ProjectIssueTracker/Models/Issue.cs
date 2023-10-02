using System.ComponentModel.DataAnnotations;

namespace ProjectIssueTracker.Models
{
    public class Issue
    {
        [Key]
        public int Id{ get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public  List<Comment> Comments { get; set; }
        public int CreatorId { get; set; }
        public User Creator { get; set; }
    }
}
