namespace ProjectIssueTracker.Dtos
{
    public class ProjectDto
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public string Description { get; set; }
        public string OwnerName { get; set; }
        public List<CollaboratorDto> Collaborators { get; set; }
        public List<IssueDto> Issues { get; set; }
    }
}
