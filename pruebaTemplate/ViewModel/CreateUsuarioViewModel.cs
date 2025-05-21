
// Models/ViewModels/CreateUsuarioViewModel.cs
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.ViewModel
{
    public class CreateUsuarioViewModel : EditUsuarioViewModel
    {
        [Required, DataType(DataType.Password), MinLength(8)]
        [DisplayName("Password")]
        public string Password { get; set; }

        [Required, DataType(DataType.Password), Compare("Password")]
        [DisplayName("Confirmar Password")]
        public string ConfirmPassword { get; set; }
    }
}