using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Bacs.Archive.Web.Models;
using Bacs.Archive.Web.Models.ManageViewModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bacs.Archive.Web.Controllers
{
    [Authorize(Roles="Admin")]
    public class ManageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ManageController(
          UserManager<ApplicationUser> userManager,
          SignInManager<ApplicationUser> signInManager,
          RoleManager<IdentityRole> roleManager,
          IOptions<IdentityCookieOptions> identityCookieOptions,
          ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        //
        // GET: /Manage/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var verifiedRoleId = await GetVerifiedRoleId();
            var userModels = _userManager
                .Users
                .Include(x => x.Roles)
                .ToList() // Required due to bug.
                .Select(x => new IndexViewModel.UserModel { Id = x.Id, UserName = x.UserName, IsVerified = x.Roles.Any(r => r.RoleId == verifiedRoleId) })
                .ToList();

            var model = new IndexViewModel { Users = userModels };
            return View(model);
        }

        private async Task<string> GetVerifiedRoleId()
        {
            var verifiedRole = await _roleManager.FindByNameAsync("User").ConfigureAwait(true);
            return verifiedRole.Id;
        }

        [HttpGet]
        public async Task<ActionResult> ChangeRole(string id)
        {
            var user = _userManager.Users.Where(x => x.Id == id).Include(x => x.Roles).First();
            var verifiedRoleId = await GetVerifiedRoleId();

            var model = new ChangeRoleModel() { Id = user.Id, UserName = user.UserName, IsVerified = user.Roles.Any(r => r.RoleId == verifiedRoleId) };
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> ChangeRole(ChangeRoleModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (model.IsVerified)
                await _userManager.AddToRoleAsync(user, "User");
            else
                await _userManager.RemoveFromRoleAsync(user, "User");
            await _userManager.UpdateSecurityStampAsync(user);
            return RedirectToAction("Index");
        }
    }
}
