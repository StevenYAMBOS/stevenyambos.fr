using Microsoft.AspNetCore.Identity;
using Portfolio.Enums;

namespace Portfolio.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public Role Role { get; set; } = Role.User;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLogin { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}