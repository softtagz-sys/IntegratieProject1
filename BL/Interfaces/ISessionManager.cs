using BL.Domain;
using BL.Domain.Answers;
using BL.Domain.Questions;

namespace BL.Interfaces;

public interface ISessionManager : IManager<Session>
{
    public Session GetSessionById(int id);

    public void AddAnswerToSession(int sessionId, Answer answer, FlowType flowType);
    public IEnumerable<Answer> GetAnswersBySessionId(int sessionId);
    Task<Session> CreateNewSession(int flowId);
    IEnumerable<Question> GetQuestionsBySessionId(int sessionId);
    public Answer UpdateAnswer(Answer answerToUpdate, Answer answer);
    Answer GetAnswerByQuestionId(int sessionId, int questionId);
}