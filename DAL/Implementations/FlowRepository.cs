using BL.Domain;
using BL.Domain.Questions;
using DAL.EF;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Implementations;

public class FlowRepository(PhygitalDbContext context) : Repository(context), IFlowRepository
{
    private readonly DbContext _context = context;
    
    public IEnumerable<Flow> GetFlowsByParentId(int? flowId)
    {
        return _context.Set<Flow>().Where(f => f.ParentFlowId == flowId);
    }
    public Flow GetFlowById(int id)
    {
        return _context.Set<Flow>().Find(id);
    }
    public async Task<IEnumerable<Question>> GetQuestionsByFlowIdAsync(int id)
    {
        return await _context.Set<Question>()
            .Where(q => q.FlowId == id)
            .Include(q => q.Options)
            .OrderBy(question => question.Position)
            .ToListAsync();
    }

    public IEnumerable<Flow> GetFlowsBetweenPositions(int newPosition, int oldPosition)
    {
        return _context.Set<Flow>().Where(f => f.Position >= oldPosition && f.Position <= newPosition);
    }
} 