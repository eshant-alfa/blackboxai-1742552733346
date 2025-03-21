using Microsoft.AspNetCore.Identity;

namespace CampusConnect.Models;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Department { get; set; }
    public string? StudentId { get; set; }
    public DateTime JoinDate { get; set; } = DateTime.UtcNow;
}