using Microsoft.AspNetCore.Identity;

namespace MiradorHRM.Models
{
    public class ApplicationRole : IdentityRole
    {
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        
    }
}
