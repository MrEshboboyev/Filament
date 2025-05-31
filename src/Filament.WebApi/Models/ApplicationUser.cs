using Microsoft.AspNetCore.Identity;

namespace Filament.WebApi.Models;


public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<FileModel> Files { get; set; } = [];
}
