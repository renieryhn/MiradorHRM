using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlanillaPM.Models
{
    public class Usuario: IdentityUser
    {
        public string Email { get; set; }
        [DisplayName("Fotografía")]
        public byte[]? Avatar { get; set; } = null!;

        [NotMapped]
        public string? AvatarBase64 { get; set; } = null!;
    }
}
