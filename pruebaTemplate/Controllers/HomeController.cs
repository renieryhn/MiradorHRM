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
using Syncfusion.Pdf.Graphics;
using Syncfusion.Drawing;
using Syncfusion.DocIORenderer;
using DocumentFormat.OpenXml.Vml.Office;


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
        public async Task<IActionResult> CreateDocument()
        {
            // Crear un nuevo documento PDF
            PdfDocument document = new PdfDocument();

            // Agregar una página al documento con tamaño A4
            PdfPage page = document.Pages.Add();

            // Set the standard font
            PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12);

            // Load image
            string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string imagePath = Path.Combine(webRootPath, "img", "Constancia1.png");
            using (FileStream fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
            {
                PdfBitmap image = new PdfBitmap(fileStream);

                // Get image dimensions
                float imageWidth = image.Width * 0.4f; // Reducir el ancho de la imagen a la mitad
                float imageHeight = image.Height * 0.4f; // Reducir la altura de la imagen a la mitad

                // Calculate center position for the image horizontally
                float centerX = (page.Size.Width - imageWidth) / 2;

                // Draw the image
                page.Graphics.DrawImage(image, new RectangleF(centerX, 50, imageWidth, imageHeight));

                // Draw the additional content
                string additionalContent = @"
C O N S T A N C I A
La suscrita Directora de Proyecto Mirador Honduras LLC (La Organización), hace constar que: El ciudadano XXXXXX, mayor de edad, unión libre, Maestro de educación primaria con número de identidad, 0000-000-000 y con domicilio en el municipio de Santa Ana, Departamento de la Paz, Presto sus servicios a PROYECTO MIRADOR desde el 03 de Mayo de 2022 hasta el 31 de Mayo del 2023. 

Y para los fines que el interesado estime conveniente se extiende la presente en la ciudad de Santa Bárbara, a los 02 día del mes de Febrero del año 2024. 

____________________________
Emilia Girón de Mendoza
Directora de Proyecto Mirador LLC (La Organización)
Honduras.
";

                // Draw the additional content
                float additionalContentWidth = 500f; // Anchura del texto adicional
                PdfTextElement textElement = new PdfTextElement(additionalContent, font);
                textElement.Brush = PdfBrushes.Black;
                textElement.Font = font;
                textElement.StringFormat = new PdfStringFormat() { Alignment = PdfTextAlignment.Justify }; // Alineación justificada
                textElement.Draw(page, new RectangleF(0, 60 + imageHeight + 20, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height)); // Dibujar el texto justificado


                // Guardar el PDF en MemoryStream
                MemoryStream stream = new MemoryStream();
                document.Save(stream);

                // Establecer la posición en '0'
                stream.Position = 0;

                // Descargar el documento PDF en el navegador
                FileStreamResult fileStreamResult = new FileStreamResult(stream, "application/pdf");
                fileStreamResult.FileDownloadName = "Constancia.pdf";

                return fileStreamResult;
            }
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
