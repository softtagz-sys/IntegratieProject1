using System.Collections;
using BL.Domain.Answers;

namespace DAL.Interfaces;

public interface IAnswerRepository : IRepository
{
    public Task<IEnumerable<Answer>> GetAnswersByQuestionIdAsync(int questionId);
}