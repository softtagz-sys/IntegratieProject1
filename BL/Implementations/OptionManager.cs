using BL.Domain.Questions;
using BL.Interfaces;
using DAL.Interfaces;

namespace BL.Implementations;

public class OptionManager(IOptionRepository repository, IQuestionManager questionManager): Manager<Option>(repository), IOptionManager
{
    public void AddOptionToQuestion(int id, string option)
    {
        repository.AddOptionToQuestion(id, option);
    }

    public bool UpdateOption(int questionId, int optionId, int? nextQuestionId)
    {
        if (nextQuestionId.HasValue && nextQuestionId != -1)
        {
            var question = questionManager.GetQuestionById(questionId);
            var nextQuestion = questionManager.GetQuestionById(nextQuestionId.Value);
            if (question.FlowId != nextQuestion.FlowId)
                return false;
        }
        return repository.UpdateOption(optionId, nextQuestionId);
    }

    public IEnumerable<Option> GetOptionsSingleOrMultipleChoiceQuestion(int id)
    {
        return repository.GetOptionsSingleOrMultipleChoiceQuestion(id);
    }

    public void UpdateOptionsAfterQuestionDeletion(int deletedQuestionId)
    {
        repository.UpdateOptionsAfterQuestionDeletion(deletedQuestionId);
    }
}