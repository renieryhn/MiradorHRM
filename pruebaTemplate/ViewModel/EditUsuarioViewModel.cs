// Models/ViewModels/EditUsuarioViewModel.cs
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.ViewModel
{
    public class EditUsuarioViewModel
    {
        public string Id { get; set; }

        [EmailAddress]
        [DisplayName("Email")]
        [Required(ErrorMessage = "El Email es obligatorio.")]
        public string Email { get; set; }

        [DisplayName("Activo")]
        public bool Activo { get; set; }
      
        [DisplayName("UserName")]
        [Required(ErrorMessage = "El UserName es obligatorio.")]
        public string UserName { get; set; }

        [DisplayName("Email confirmado")]
        public bool EmailConfirmed { get; set; }

        [Phone]
        [DisplayName("Teléfono")]
        public string PhoneNumber { get; set; }

        [DisplayName("2FA habilitado")]
        public bool TwoFactorEnabled { get; set; }

        //[DisplayName("Unidad")]
        //public int? IdUnidad { get; set; }

        // lista para el select
        //public List<SelectListItem> UnidadesSelect { get; set; }

        [DisplayName("Nombre completo")]
        public string NombreCompleto { get; set; }

        //[DisplayName("Carga de trabajo")]
        //public bool CargaTrabajo { get; set; }
    }
}

