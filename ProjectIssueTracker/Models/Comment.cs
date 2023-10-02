using System.ComponentModel.DataAnnotations;

namespace ProjectIssueTracker.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int CommenterId { get; set; }
        public User Commenter { get; set; }
        public int IssueId { get; set; }
        public Issue Issue { get; set; }
    }
}
