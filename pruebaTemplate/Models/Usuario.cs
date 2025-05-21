using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlanillaPM.Models
{
    public class Usuario: IdentityUser
    {
        //[DisplayName("Nombre de Usuario")]
        //public string UserName { get; set; }
        [DisplayName("Nombre")]
        public string NombreCompleto { get; set; }

        [DisplayName("Activo")]
        public bool Activo { get; set; }

        [DisplayName("Email")]
        public string Email { get; set; }
        [DisplayName("Fotografía")]

        public byte[]? Avatar { get; set; } = null!;
        public string? AvatarName { get; set; } = null!;
        public string? AvatarPath { get; set; } = null!;

        [NotMapped]
        public string? AvatarBase64 { get; set; } = null!;


    }
}
