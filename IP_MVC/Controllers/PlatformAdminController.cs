using BL.Interfaces;
using IP_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace IP_MVC.Controllers
{
    public class PlatformAdmin : Controller
    {
        private readonly IProjectManager _projectManager;
        private readonly IUserManager _userManager;

        public PlatformAdmin(IProjectManager projectManager, IUserManager userManager)
        {
            _projectManager = projectManager;
            _userManager = userManager;
        }

        // GET
        public async Task<IActionResult> PlatformAdminDashboard()
        {
            var projects = await _projectManager.GetAllAsync();
            var projectViewModels = projects.Select(p => new ProjectViewModel
            {
                Project = p,
                Owner = _userManager.GetUserById(p.AdminId)
            });
            return View(projectViewModels);
        }
    }
}