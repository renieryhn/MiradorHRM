using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.ViewModel
{
    public class UsuarioListadoViewModel
    {
      
            public string Id { get; set; }

            [Display(Name = "Usuario")]
            public string UserName { get; set; }
        [DisplayName("Activo")]
        public bool Activo { get; set; }

        [Display(Name = "Email")]
            public string Email { get; set; }

            [Display(Name = "Nombre completo")]
            public string NombreCompleto { get; set; }

            [Display(Name = "Unidad")]
            public string UnidadNombre { get; set; }

            [Display(Name = "Teléfono")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Email confirmado")]
            public bool EmailConfirmed { get; set; }

            [Display(Name = "2FA habilitado")]
            public bool TwoFactorEnabled { get; set; }

            [Display(Name = "Carga trabajo")]
            public bool CargaTrabajo { get; set; }        

    }
}
