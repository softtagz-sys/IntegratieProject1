using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IP_MVC.Controllers;

public class FacilitatorController : Controller
{
    // GET
    [Authorize(Roles = CustomIdentityConstants.FacilitatorRole)]
    public IActionResult FacilitatorDashboard()
    {
        return View();
    }
}