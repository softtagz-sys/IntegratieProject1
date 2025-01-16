using System.Security.Claims;
using BL.Domain;
using BL.Implementations;
using BL.Interfaces;
using IP_MVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace IP_MVC.Controllers;

public class ProjectController : Controller
{
    private readonly IProjectManager _projectManager;
    private readonly IFlowManager _flowManager;
    private readonly IQuestionManager _questionManager;
    private readonly IOptionManager _optionManager;
    private readonly UnitOfWork _unitOfWork;

    public ProjectController(IProjectManager projectManager, IFlowManager flowManager, IQuestionManager questionManager, IOptionManager optionManager, UnitOfWork unitOfWork)
    {
        _projectManager = projectManager;
        _flowManager = flowManager;
        _questionManager = questionManager;
        _optionManager = optionManager;
        _unitOfWork = unitOfWork;
    }


    public async Task<IActionResult> ProjectDashboard()
    {
        ViewBag.Dashboard = true;
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var adminProjects = await _projectManager.GetProjectsByAdminIdAsync(userId);
        var facilitatorProjects = _projectManager.GetProjectsByFacilitatorId(userId);

        var viewModel = new ProjectDashboardViewModel
        {
            AdminProjects = adminProjects,
            FacilitatorProjects = facilitatorProjects
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Delete(int parentFlowId)
    {
        _unitOfWork.BeginTransaction();
        
        var project = await _projectManager.FindByIdAsync(parentFlowId);

        var flows = _projectManager.GetFlowsByProjectIdAsync(parentFlowId).Result;
        foreach (var flow in flows)
        {
            var questions = _questionManager.GetQuestionsByFlowId(flow.Id);
            if (questions != null)
            {
                foreach (var question in questions)
                {
                    var options = _optionManager.GetOptionsSingleOrMultipleChoiceQuestion(question.Id);
                    if (options != null)
                    {
                        foreach (var option in options)
                        {
                            await _optionManager.DeleteAsync(option);
                        }
                    }
                    await _questionManager.DeleteAsync(question);

                    _questionManager.RemoveAnswersByQuestionId(question.Id);
                }
            }
            await _flowManager.DeleteAsync(flow);
        }
        await _projectManager.DeleteAsync(project);

        _unitOfWork.Commit();
        return RedirectToAction("ProjectDashboard");
    }

    [HttpPost]
    public async Task<IActionResult> Create(Project project)
    {
        _unitOfWork.BeginTransaction();
        if (!ModelState.IsValid) return RedirectToAction("ProjectDashboard");

        project.AdminId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        await _projectManager.AddAsync(project);

        _unitOfWork.Commit();
        return RedirectToAction("ProjectDashboard");
    }


    [HttpGet]
    public IActionResult ManageFacilitators(int projectId)
    {
        var facilitators = _projectManager.GetFacilitatorsByProjectId(projectId);
        
        var model = new ManageFacilitatorsViewModel
        {
            Facilitators = facilitators,
            ProjectId = projectId
        };

        return View(model);
    }


    public IActionResult RemoveUser(string userId, int projectId)
    {
        _projectManager.RemoveFacilitatorFromProject(userId, projectId);
        return RedirectToAction("ManageFacilitators", new { projectId });
    }
}