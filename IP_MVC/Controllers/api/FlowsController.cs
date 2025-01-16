using BL.Domain;
using BL.Implementations;
using BL.Interfaces;
using IP_MVC.Models;
using IP_MVC.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Project = BL.Domain.Project;

namespace IP_MVC.Controllers.api;

[ApiController]
[Route("api/[controller]")]
public class FlowsController : ControllerBase    
{
    private readonly IFlowManager _flowManager;
    private readonly UnitOfWork _unitOfWork;

    public FlowsController(IFlowManager flowManager, UnitOfWork unitOfWork)
    {
        _flowManager = flowManager;
        _unitOfWork = unitOfWork;
    }

    [HttpGet("{flowId}")]
    public Task<IActionResult> GetFlow(int flowId)
    {
        var flow = _flowManager.GetFlowById(flowId);

        if (flow == null)
        {
            return Task.FromResult<IActionResult>(NotFound());
        }

        return Task.FromResult<IActionResult>(Ok(flow));
    }
    
    [HttpPut]
    public async Task<IActionResult> Change([FromBody] FlowEditDto updateDto)
    {
        _unitOfWork.BeginTransaction();
        if (updateDto == null)
        {
            return BadRequest("Invalid flow data.");
        }

        var flow =  _flowManager.GetFlowById(updateDto.Id);

        var updatedFlow = flow;
        updatedFlow.Name = updateDto.NewName;
        updatedFlow.Description = updateDto.NewDescription;
        
        await _flowManager.UpdateAsync( flow, updatedFlow);
        _unitOfWork.Commit();
        return NoContent(); 
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] FlowCreateDto createDto)
    {
        _unitOfWork.BeginTransaction();
        if (createDto == null)
        {
            return BadRequest("Invalid flow data.");
        }

        Flow newFlow = new Flow
        {
            Name = createDto.NewName,
            Description = createDto.NewDescription,
            StartDate = createDto.NewStartDate.ToUniversalTime(),
            EndDate = createDto.NewEndDate.ToUniversalTime(),
            ProjectId = createDto.NewProjectId,
            ParentFlowId = createDto.NewParentFlowId
        };

      
        await _flowManager.AddAsync(newFlow); 
        
        _unitOfWork.Commit();
       
        return Ok(newFlow);
    }

    [HttpGet("{flowId}/SubFlows")]
    public IActionResult GetAllSubFlows(int flowId)
    {
        var subFlows = _flowManager.GetFlowsByParentId(flowId);
        return Ok(subFlows);
    }
    
    [HttpPost("SetPlayerCount")]
    public async Task<IActionResult> SetPlayerCount([FromBody] PlayerCountModel model)
    {
        HttpContext.Session.SetInt32("playerCount", model.PlayerCount);
        return await Task.FromResult<IActionResult>(Ok());
    }
}