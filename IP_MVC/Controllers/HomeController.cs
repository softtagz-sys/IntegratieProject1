using System.Diagnostics;
using BL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using net6npmwebpack.Models;
using WebApplication1.Models;

namespace IP_MVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProjectManager _projectManager;

    public HomeController(ILogger<HomeController> logger, IProjectManager projectManager)
    {
        _logger = logger;
        _projectManager = projectManager;
    }

    public async Task<IActionResult> Index()
    {
        var projects = await _projectManager.GetAllAsync();
        return View(projects);
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

    [HttpGet]
    public IActionResult Contact()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Contact(ContactViewModel contactVM)
    {
        if (ModelState.IsValid)
        {
            // Send an email or save the message in a table...
            // Redirect to a page that says "Thanks for contacting us!"...

            return RedirectToAction("Index");
        }

        return View();
    }
}