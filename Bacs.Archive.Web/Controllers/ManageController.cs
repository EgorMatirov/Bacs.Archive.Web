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
        private const string UserRoleName = "User";
        private const string MaintainerRoleName = "Maintainer";
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

        private async Task<string> GetRoleId(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName).ConfigureAwait(true);
            return role.Id;
        }

        private Task<string> GetVerifiedRoleId()
        {
            return GetRoleId(UserRoleName);
        }

        private Task<string> GetMaintainerRoleId()
        {
            return GetRoleId(MaintainerRoleName);
        }

        private async Task UpdateRoleStatus(ApplicationUser user, string roleName, bool condition)
        {
            if (condition)
                await _userManager.AddToRoleAsync(user, roleName);
            else
                await _userManager.RemoveFromRoleAsync(user, roleName);
        }

        [HttpGet]
        public async Task<ActionResult> ChangeRole(string id)
        {
            var user = _userManager.Users.Where(x => x.Id == id).Include(x => x.Roles).First();
            var verifiedRoleId = await GetVerifiedRoleId();
            var maintainerRoleId = await GetMaintainerRoleId();

            var model = new ChangeRoleModel
            {
                Id = user.Id,
                UserName = user.UserName,
                IsVerified = user.Roles.Any(r => r.RoleId == verifiedRoleId),
                CanUpload = user.Roles.Any(r => r.RoleId == maintainerRoleId)
            };
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> ChangeRole(ChangeRoleModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            await UpdateRoleStatus(user, UserRoleName, model.IsVerified);
            await UpdateRoleStatus(user, MaintainerRoleName, model.CanUpload);
            await _userManager.UpdateSecurityStampAsync(user);
            return RedirectToAction("Index");
        }
    }
}
