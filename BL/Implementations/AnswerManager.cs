using BL.Domain.Answers;
using BL.Interfaces;
using DAL.Interfaces;

namespace BL.Implementations;

public class AnswerManager(IAnswerRepository repository) : Manager<Answer>(repository), IAnswerManager
{
    public async Task<IEnumerable<Answer>> GetAnswersByQuestionIdAsync(int questionId)
    {
        return await repository.GetAnswersByQuestionIdAsync(questionId);
    }
}