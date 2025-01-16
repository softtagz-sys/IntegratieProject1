using BL.Domain.Answers;
using BL.Domain.Questions;

namespace BL.Interfaces;

public interface IAnalyticsManager
{
    public object GetSingleChoiceQuestionData(SingleChoiceQuestion question, IEnumerable<Answer> answers);
    public object GetMultipleChoiceQuestionData(MultipleChoiceQuestion question, IEnumerable<Answer> answers);
    public object GetRangeQuestionData(RangeQuestion question, IEnumerable<Answer> answers);
    public object GetOpenQuestionData(OpenQuestion question, IEnumerable<Answer> answers);
}