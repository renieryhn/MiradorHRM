using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanillaPM.Models;

namespace PlanillaPM.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        public UsersController(UserManager<Usuario> userManager)
        {
            _userManager = userManager;
        }
       
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var allUsersExceptCurrentUser = await _userManager.Users.Where(u => u.Id != currentUser.Id).ToListAsync();
            return View(allUsersExceptCurrentUser);
        }

    }
}
