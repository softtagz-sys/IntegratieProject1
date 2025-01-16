using BL.Domain;
using BL.Interfaces;
using DAL.Interfaces;
using BL.Domain.Answers;
using BL.Domain.Questions;

namespace BL.Implementations;

public class SessionManager(ISessionRepository repository, IQuestionManager questionManager) : Manager<Session>(repository), ISessionManager
{
    public Session GetSessionById(int id)
    {
        return repository.GetSessionById(id);
    }

    public void AddAnswerToSession(int sessionId, Answer answer, FlowType flowType)
    {
        repository.AddAnswerToSession(sessionId, answer, flowType);
    }

    public IEnumerable<Answer> GetAnswersBySessionId(int sessionId)
    {
        return repository.GetAnswersBySessionId(sessionId);
    }

    public async Task<Session> CreateNewSession(int flowId)
    {
        var newSession = new Session
        {
            FlowId = flowId,
            Answers = new List<Answer>()
        };

        await AddAsync(newSession);

        return newSession;
    }

    public IEnumerable<Question> GetQuestionsBySessionId(int sessionId)
    {
        var answers = GetAnswersBySessionId(sessionId).ToList();
        return answers.Select(a => questionManager.GetQuestionById(a.QuestionId)).ToList();
    }
    public Answer UpdateAnswer(Answer oldAnswer, Answer newAnswer)
    {
        if (oldAnswer == null)
        {
            AddAnswerToSession(newAnswer.Session.Id, newAnswer, FlowType.LINEAR);
        }
        else
        {
            oldAnswer.AnswerTextPlayer1 = newAnswer.AnswerTextPlayer1; 
            oldAnswer.AnswerTextPlayer2 = newAnswer.AnswerTextPlayer2; 
            repository.UpdateAnswer(oldAnswer, newAnswer);
        }
        return oldAnswer;
    }

    public Answer GetAnswerByQuestionId(int sessionId, int questionId)
    {
        return repository.GetAnswerByQuestionId(sessionId, questionId);
    }
}