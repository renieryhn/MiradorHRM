using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models
{
    public class Registro
    {


        [Required(ErrorMessage = "El número de teléfono es obligatorio")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [EmailAddress(ErrorMessage = "El campo debe ser un correo electronico valido")]
        public string Email { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [DataType(DataType.Password)]
        public string RetypePassword { get; set; }
        public string ReturnUrl { get; set; }
    }
}
