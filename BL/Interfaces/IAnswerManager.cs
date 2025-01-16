using BL.Domain.Answers;

namespace BL.Interfaces;

public interface IAnswerManager : IManager<Answer>
{
    public Task<IEnumerable<Answer>> GetAnswersByQuestionIdAsync(int questionId);
}