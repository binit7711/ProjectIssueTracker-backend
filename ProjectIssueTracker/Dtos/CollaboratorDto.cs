namespace ProjectIssueTracker.Dtos
{
    public class CollaboratorDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int ProjectId { get; set; }
    }
}
