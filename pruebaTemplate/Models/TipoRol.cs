using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PlanillaPM.Models
{
    public class TipoRol 
    {
        public string Id { get; set; }
        [Display(Name = "Rol")]
        public string Name { get; set; }
        
    }
}
