using BL.Domain;
using BL.Domain.Questions;
using BL.Interfaces;
using DAL.EF;
using DAL.Implementations;
using DAL.Interfaces;


namespace BL.Implementations;

public class FlowManager(IFlowRepository repository,ISessionManager sessionManager)
    : Manager<Flow>(repository), IFlowManager
{
    public IEnumerable<Flow> GetFlowsByParentId(int? flowId)
    {
        return repository.GetFlowsByParentId(flowId);
    }

    public Flow GetFlowById(int id)
    {
        return repository.GetFlowById(id);
    }

    public async Task<IEnumerable<Question>> GetQuestionsByFlowIdAsync(int flowId)
    {
        return await repository.GetQuestionsByFlowIdAsync(flowId);
    }

    public async Task<Queue<int>> GetQuestionQueueByFlowIdAsync(int flowId)
    {
        var questionIds = (await GetQuestionsByFlowIdAsync(flowId)).OrderBy(q => q.Position).Select(q => q.Id).ToList();
        return new Queue<int>(questionIds);
    }

    public int? GetParentFlowIdBySessionId(int sessionId)
    {
        var flow = GetFlowById(sessionManager.GetSessionById(sessionId).FlowId);
        return flow.ParentFlowId;
    }

    public bool IsParentFlow(int flowId)
    {
        var allSubFlows = GetFlowsByParentId(flowId);
        return allSubFlows?.Any() ?? false;
    }

    public IEnumerable<Flow> GetFlowsBetweenPositions(int newPosition, int oldPosition)
    {
        return repository.GetFlowsBetweenPositions(newPosition, oldPosition);
    }
}