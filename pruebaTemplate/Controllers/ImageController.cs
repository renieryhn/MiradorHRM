using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanillaPM.Models;
using PlanillaPM.ViewModel;

namespace PlanillaPM.Controllers
{
    public class ImageController : Controller
    {
        private readonly PlanillaContext _context;

        public ImageController(PlanillaContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file, int idEmpleado)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var empleado = await _context.Empleados.FindAsync(idEmpleado);
            if (empleado == null)
                return NotFound();

            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Imagenes", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            empleado.FotografiaNombre = fileName;
            empleado.FotografiaPath = "/Imagenes/" + fileName;
            empleado.FotoTmp = file;

            _context.Entry(empleado).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Empleado", new { id = idEmpleado });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveImage(int idEmpleado)
        {
            var empleado = await _context.Empleados.FindAsync(idEmpleado);
            if (empleado == null)
                return NotFound();

            if (!string.IsNullOrEmpty(empleado.FotografiaNombre))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Imagenes", empleado.FotografiaNombre);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                empleado.FotografiaNombre = null;
                empleado.FotografiaPath = null;
                empleado.FotoTmp = null;

                _context.Entry(empleado).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", "Empleado", new { id = idEmpleado });
        }
    }
}