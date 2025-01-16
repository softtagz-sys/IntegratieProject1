using BL.Domain;
using BL.Domain.Answers;

namespace DAL.Interfaces;

public interface ISessionRepository : IRepository
{
    Session GetSessionById(int id);
    void AddAnswerToSession(int sessionId, Answer answer, FlowType flowType);
    IEnumerable<Answer> GetAnswersBySessionId(int sessionId);
    Answer UpdateAnswer(Answer answerToUpdate, Answer answer);
    Answer GetAnswerByQuestionId(int sessionId, int questionId);
}