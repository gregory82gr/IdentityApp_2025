using Microsoft.AspNetCore.Identity;

namespace API.Models
{
    public class AppUserRoleBridge:IdentityUserRole<int>
    {
        // Navigation properties
        public AppUser User { get; set; }
        public AppRole Role { get; set; }
    }
}
