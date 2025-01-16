using BL.Domain.Questions;
using DAL.EF;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Implementations;

public class OptionRepository(PhygitalDbContext context) : Repository(context), IOptionRepository
{
    private readonly DbContext _context = context;
    
    public void AddOptionToQuestion(int id, string option)
    {
        var singleChoiceQuestion = _context.Set<SingleChoiceQuestion>().FirstOrDefault(q => q.Id == id);
        if (singleChoiceQuestion != null) 
        {
            singleChoiceQuestion.Options ??= new List<Option>();
            singleChoiceQuestion.Options.Add(new Option() {Text = option});
        }
        else
        {
            var multipleChoiceQuestion = _context.Set<MultipleChoiceQuestion>().FirstOrDefault(q => q.Id == id);
            if (multipleChoiceQuestion != null)
            {
                multipleChoiceQuestion.Options ??= new List<Option>();
                multipleChoiceQuestion.Options.Add(new Option() {Text = option});
            }
        }
    }
    public bool UpdateOption(int optionId, int? nextQuestionId)
    {
        var option = _context.Set<Option>().FirstOrDefault(o => o.Id == optionId);
        if (option == null) return false;
        option.NextQuestionId = nextQuestionId;
        return true;
    }
    
    public IEnumerable<Option> GetOptionsSingleOrMultipleChoiceQuestion(int id)
    {
        var singleChoiceOptions = _context.Set<SingleChoiceQuestion>().Include(q => q.Options).FirstOrDefault(q => q.Id == id)?.Options;
        return singleChoiceOptions ?? _context.Set<MultipleChoiceQuestion>().Include(q => q.Options).FirstOrDefault(q => q.Id == id)?.Options;
    }

    public void UpdateOptionsAfterQuestionDeletion(int deletedQuestionId)
    {
        var allOptions = _context.Set<Option>().ToList();

        foreach (var option in allOptions)
        {
            if (option.NextQuestionId == deletedQuestionId)
            {
                option.NextQuestionId = null;
            }
        }
    }
    
}