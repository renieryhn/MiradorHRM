namespace MiradorHRM.Models
{
    public class VentanaPermisosViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool Ver { get; set; }
        public bool Crear { get; set; }
        public bool Editar { get; set; }
        public bool Eliminar { get; set; }
    }
}
