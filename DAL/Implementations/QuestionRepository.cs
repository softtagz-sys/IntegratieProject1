using BL.Domain;
using BL.Domain.Answers;
using BL.Domain.Questions;
using DAL.EF;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Implementations;

public class QuestionRepository(PhygitalDbContext context) : Repository(context), IQuestionRepository
{
    private readonly DbContext _context = context;

    public Question GetQuestionByIdWithMedia(int questionId)
    {
        return
            _context.Set<Question>().Include(q => q.Media).Include(q => q.Options).FirstOrDefault(q => q.Id == questionId);
    }

    public IEnumerable<Question> GetQuestionsByFlowId(int flowId)
    {
        return _context.Set<Question>().Where(q => q.FlowId == flowId).Include(q => q.Media).Include(q => q.Options).ToList();
    }
    
    public IEnumerable<Question> GetQuestionsByFlowIdWithMedia(int id)
    {
        return _context.Set<Question>().Where(q => q.FlowId == id).Include(q => q.Media).Include(q => q.Options).ToList();
    }

    public IEnumerable<Question> GetQuestionsBetweenPositionsByFlowId(int flowId, int newPosition, int oldPosition)
    {
        return _context.Set<Question>().Where(q => q.FlowId == flowId && q.Position >= newPosition && q.Position <= oldPosition).ToList();
    }
    public (int, int) GetRangeQuestionValues(int id)
    {
        var rangeQuestion = _context.Set<RangeQuestion>().FirstOrDefault(q => q.Id == id);
        return rangeQuestion != null ? (rangeQuestion.Min, rangeQuestion.Max) : (0, 0);
    }

    

    public void SetRangeQuestionValues(int id, int min, int max)
    {
        var rangeQuestion = _context.Set<RangeQuestion>().FirstOrDefault(q => q.Id == id);
        if (rangeQuestion == null) return;
        rangeQuestion.Min = min;
        rangeQuestion.Max = max;
        _context.SaveChanges();
    }
    

    public void AddMediaToQuestion(int questionId, string path, string description, MediaType type)
    {
        var media = new Media()
        {
            url = path,
            description = description,
            type = type
        };
        _context.Set<Question>().FirstOrDefault(q => q.Id == questionId)!.Media = media;
    }

    public IEnumerable<Question> GetQuestionsByFlowIdAfterPosition(int flowId, int position)
    {
        return _context.Set<Question>().Where(q => q.FlowId == flowId && q.Position > position);
    }
    
    public void RemoveAnswersByQuestionId(int questionId)
    {
        var answers = _context.Set<Answer>().Where(a => a.QuestionId == questionId);
        _context.Set<Answer>().RemoveRange(answers);
    }
    
    public async Task<IEnumerable<Note>> GetNotesByQuestionAsync(int questionId)
    {
        var notes = await _context.Set<Note>()
            .Where(n => n.QuestionId == questionId)
            .ToListAsync();

        return notes;
    }
}