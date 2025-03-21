namespace CampusConnect.Models;

public class NewsPost
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime PublishDate { get; set; } = DateTime.UtcNow;
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }
}