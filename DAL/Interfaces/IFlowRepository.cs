using System.Collections;
using BL.Domain;
using BL.Domain.Answers;
using BL.Domain.Questions;

namespace DAL.Interfaces;

public interface IFlowRepository : IRepository
{
    public IEnumerable<Flow> GetFlowsByParentId(int? flowId);
    public Flow GetFlowById(int id);
    public Task<IEnumerable<Question>> GetQuestionsByFlowIdAsync(int id);
    public IEnumerable<Flow> GetFlowsBetweenPositions(int newPosition, int oldPosition);
}