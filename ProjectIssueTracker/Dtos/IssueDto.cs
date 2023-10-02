namespace ProjectIssueTracker.Dtos
{
    public class IssueDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int CreatorId { get; set; }
        public int ProjectId { get; set; }
        public List<CommentDto> Comments{ get; set; }
    }
}
