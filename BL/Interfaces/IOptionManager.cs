using BL.Domain.Questions;

namespace BL.Interfaces;

public interface IOptionManager:IManager<Option>
{
    public void AddOptionToQuestion(int id, string option);
    public bool UpdateOption(int questionId, int optionId, int? nextQuestionId);
    public IEnumerable<Option> GetOptionsSingleOrMultipleChoiceQuestion(int id);
    public void UpdateOptionsAfterQuestionDeletion(int deletedQuestionId);
}