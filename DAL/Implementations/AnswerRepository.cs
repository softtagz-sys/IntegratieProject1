using BL.Domain.Answers;
using DAL.EF;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Implementations
{
    public class AnswerRepository(PhygitalDbContext context) : Repository(context), IAnswerRepository
    {
        private readonly PhygitalDbContext _context = context;

        public async Task<IEnumerable<Answer>> GetAnswersByQuestionIdAsync(int questionId)
        {
            return await _context.Set<Answer>()
                .Where(a => a.QuestionId == questionId)
                .ToListAsync();
        }
    }
}