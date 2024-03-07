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
            var prefix = "data:image/jpeg;base64,";
           

            // Crear un objeto ProfileViewModel con los datos del usuario
            var profileViewModel = new ProfileViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Avatar = user.Avatar,
            };
            if (user.Avatar != null)
            {
                profileViewModel.AvatarBase64 = prefix + Convert.ToBase64String(user.Avatar);
                user.AvatarBase64 = prefix + Convert.ToBase64String(user.Avatar);
                ViewData["Avatar64"] = profileViewModel.AvatarBase64;
            }
            else
            {
                ViewData["Avatar64"] = Url.Content("~/img/avatar.png");
            }
            return View("Index");
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
    }
}
