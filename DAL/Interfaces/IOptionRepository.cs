using BL.Domain.Questions;

namespace DAL.Interfaces;

public interface IOptionRepository : IRepository
{
    public void AddOptionToQuestion(int id, string option);
    public bool UpdateOption(int optionId, int? nextQuestionId);
    public IEnumerable<Option> GetOptionsSingleOrMultipleChoiceQuestion(int id);
    public void UpdateOptionsAfterQuestionDeletion(int deletedQuestionId);

}