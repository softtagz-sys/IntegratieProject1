using BL.Domain;
using BL.Domain.Answers;
using DAL.EF;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Implementations;

public class SessionRepository(PhygitalDbContext context) : Repository(context), ISessionRepository
{
    public Session GetSessionById(int id)
    {
        return context.Set<Session>().Find(id);
    }
    
    public void AddAnswerToSession(int sessionId, Answer answer, FlowType flowType)
    {
        var answerToRemove = context.Answers
            .FirstOrDefault(a => a.Session.Id == sessionId && a.QuestionId == answer.QuestionId);

        if (answerToRemove !=null && flowType == FlowType.LINEAR)
        { 
            context.Answers.Remove(answerToRemove);
        }
        context.Answers.Add(answer);
    }

    public void Update(Session session)
    {
        context.Attach(session).State = EntityState.Modified;
        try
        {
            context.SaveChanges();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!SessionExists(session.Id))
            {
                throw new Exception("No session found with this id");
            }
            else
            {
                throw;
            }
        }
    }

    public IEnumerable<Answer> GetAnswersBySessionId(int sessionId)
    {
        return context.Sessions.Include(s => s.Answers).FirstOrDefault(s => s.Id == sessionId)?.Answers;
    }

    public Answer UpdateAnswer(Answer answerToUpdate, Answer answer)
    {
        var session = context.Sessions.Include(s => s.Answers).FirstOrDefault(s => s.Id == answerToUpdate.Session.Id);
        if (session == null)
        {
            throw new Exception("No session found with this id");
        }

        if (session.Answers.Any(a => a.QuestionId == answer.QuestionId))
        {
            var existingAnswer = session.Answers.First(a => a.QuestionId == answer.QuestionId);
            existingAnswer.AnswerTextPlayer1 = answer.AnswerTextPlayer1; 
            existingAnswer.AnswerTextPlayer2 = answer.AnswerTextPlayer2; 
        }
        else
        {
            session.Answers.Add(answer);
        }
        context.SaveChanges();
        return answer;
    }

    private bool SessionExists(int id)
    {
        return context.Sessions.Any(e => e.Id == id);
    }

    public Answer GetAnswerByQuestionId(int sessionId, int questionId)
    {
        return context.Answers.FirstOrDefault(a => a.Session.Id == sessionId && a.QuestionId == questionId);
    }
}