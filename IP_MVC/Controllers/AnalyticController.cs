using BL.Domain;
using BL.Interfaces;
using IP_MVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace IP_MVC.Controllers;

public class AnalyticController(IProjectManager projectManager, IFlowManager flowManager) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Analytic(int projectId = 1)
    {
        var flows = await projectManager.GetFlowsByProjectIdAsync(projectId);
        var flowsWithQuestions = new List<Flow>();

        foreach (var flow in flows)
        {
            var questions = await flowManager.GetQuestionsByFlowIdAsync(flow.Id);
            if (questions.Any())
            {
                flowsWithQuestions.Add(flow);
            }
        }

        return View(flowsWithQuestions);
    }
}