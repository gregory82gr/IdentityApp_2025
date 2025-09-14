using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace API.Models
{
    public class AppRole:IdentityRole<int>
    {
        // Navigation property
        public ICollection<AppUserRoleBridge> Users { get; set; }
    }
}
