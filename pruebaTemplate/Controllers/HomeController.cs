using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlanillaPM.ViewModel;
using pruebaTemplate.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using PlanillaPM.Models;
using Newtonsoft.Json;
using System.IO;
using Microsoft.AspNetCore.Authorization;

using Syncfusion.Pdf;
using Syncfusion.DocIORenderer;
using Syncfusion.HtmlConverter;



namespace pruebaTemplate.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<Usuario> userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<Usuario> userManager)
        {
            _logger = logger;
            this.userManager = userManager;
        }
        public async Task<IActionResult> IndexAsync()
        {
            var user = await userManager.GetUserAsync(User);

            // Crear un objeto ProfileViewModel con los datos del usuario
            var profileViewModel = new ProfileViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Avatar = user.Avatar,
            };

            return View(profileViewModel);
        }


        public IActionResult Privacy()
        {
            return View();
        }

      


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

   

        [HttpPost]
        public IActionResult ConvertirWordAPdf(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    // Copiar el archivo cargado a un MemoryStream
                    file.CopyTo(ms);

                    // Convertir el documento de Word a PDF
                    using (DocIORenderer renderer = new DocIORenderer())
                    {
                        // Renderizar el documento de Word a PDF
                        PdfDocument pdfDocument = renderer.ConvertToPDF(ms);

                        // Guardar el PDF en MemoryStream
                        MemoryStream pdfStream = new MemoryStream();
                        pdfDocument.Save(pdfStream);

                        // Devolver el PDF como descarga al navegador
                        return File(pdfStream.ToArray(), "application/pdf", "documento.pdf");
                    }
                }
            }
            else
            {
                // Si no se selecciona ningún archivo, redirigir a la página de inicio
                return RedirectToAction("Index");
            }
        }


    }
}
