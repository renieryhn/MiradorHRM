using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models
{
    public class EmailViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Nuevo Correo Electrónico")]
        public string Email { get; set; }

        public bool IsEmailConfirmed { get; set; }
    }
}
