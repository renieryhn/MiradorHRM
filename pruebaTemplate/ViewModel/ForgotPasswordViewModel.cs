using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.ViewModel
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }
    }
}
