using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.ViewModel
{
    public class ProfileViewModel
    {
        [Required]
        [Display(Name = "Nombre de Usuario")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Número de Teléfono")]
        public string PhoneNumber { get; set; }
    }
}
