using System.ComponentModel.DataAnnotations.Schema;

namespace PlanillaPM.ViewModel
{
    public class ImageHandler
    {
        public string NombreModelo { get; set; }
        public string IdEnModelo { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        [NotMapped] // Esta propiedad no será mapeada a la base de datos
        public IFormFile ImageFile { get; set; }
    }
}
