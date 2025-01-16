using BL.Domain;
using BL.Domain.Questions;

namespace DAL.Interfaces;

public interface IQuestionRepository : IRepository
{
    public Question GetQuestionByIdWithMedia(int questionId);
    IEnumerable<Question> GetQuestionsByFlowId(int flowId);
    IEnumerable<Question> GetQuestionsByFlowIdWithMedia(int flowId);
    IEnumerable<Question> GetQuestionsBetweenPositionsByFlowId(int flowId, int newPosition, int oldPosition);

    public (int, int) GetRangeQuestionValues(int id);
    
    public void SetRangeQuestionValues(int id, int min, int max);
    
    public void AddMediaToQuestion(int questionId, string path, string description, MediaType type);
    public IEnumerable<Question> GetQuestionsByFlowIdAfterPosition(int flowId, int position);
    public void RemoveAnswersByQuestionId(int questionId);
    public Task<IEnumerable<Note>> GetNotesByQuestionAsync(int questionId);
}