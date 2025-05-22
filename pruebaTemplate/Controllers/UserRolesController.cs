using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlanillaPM.Models;

namespace PlanillaPM.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class UserRolesController : Controller
    {
        private readonly SignInManager<Usuario> _signInManager;
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRolesController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index(string userId)
        {

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "Usuario no encontrado.";
                return RedirectToAction("Index", "Users");
            }

            var viewModel = new List<UserRolesViewModel>();         
            foreach (var role in _roleManager.Roles.ToList())
            {
                var userRolesViewModel = new UserRolesViewModel
                {
                    RoleName = role.Name
                };
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRolesViewModel.Selected = true;
                }
                else
                {
                    userRolesViewModel.Selected = false;
                }
                viewModel.Add(userRolesViewModel);
            }
            var model = new ManageUserRolesViewModel()
            {
                UserId = userId,
                UserRoles = viewModel
            };

            return View(model);
        }

        public async Task<IActionResult> Update(string id, ManageUserRolesViewModel model)
        {
            var user = await _userManager.FindByIdAsync(id);
          
            if (user == null)
            {
                TempData["Error"] = "Usuario no encontrado.";
                return RedirectToAction("Index", "Users");
            }
            var roles = await _userManager.GetRolesAsync(user);

            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                // Si la eliminación de roles no tuvo éxito, puedes establecer un TempData de error y redirigir a la acción Index
                TempData["Error"] = "Hubo un problema al eliminar los roles del usuario.";
                return RedirectToAction("Index", new { userId = id });
            }

            result = await _userManager.AddToRolesAsync(user, model.UserRoles.Where(x => x.Selected).Select(y => y.RoleName));
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                // Si la adición de roles no tuvo éxito, puedes establecer un TempData de error y redirigir a la acción Index
                TempData["Error"] = "Hubo un problema al agregar los nuevos roles al usuario.";
                return RedirectToAction("Index", new { userId = id });
            }

            // Si todo fue exitoso, puedes establecer un TempData de éxito y redirigir a la acción Index
            TempData["success"] = "Se han actualizado los roles del usuario exitosamente.";

            var currentUser = await _userManager.GetUserAsync(User);
            await _signInManager.RefreshSignInAsync(currentUser);
            //await Seeds.DefaultUsers.SeedSuperAdminAsync(_userManager, _roleManager);

            //return RedirectToAction("Index", new { userId = id });
            return RedirectToAction("IndexUsuario", "Usuario");

        }

        
    }
}
