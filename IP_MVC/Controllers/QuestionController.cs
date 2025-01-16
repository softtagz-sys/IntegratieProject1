using BL.Domain;
using BL.Domain.Questions;
using BL.Implementations;
using Microsoft.AspNetCore.Mvc;
using BL.Interfaces;
using IP_MVC.Models;
using QRCoder;
using WebApplication1.Models;

namespace IP_MVC.Controllers
{
    public class QuestionController : Controller
    {
        private readonly IQuestionManager _questionManager;
        private readonly IOptionManager _optionManager;
        private readonly UnitOfWork _unitOfWork;

        public QuestionController(IQuestionManager questionManager, IOptionManager optionManager, UnitOfWork unitOfWork)
        {
            _questionManager = questionManager;
            _unitOfWork = unitOfWork;
            _optionManager = optionManager;
        }

        [HttpGet]
        public IActionResult Edit(int questionId)
        {
            var question = _questionManager.GetQuestionById(questionId);
            if (question == null)
            {
                var errorViewModel = new ErrorViewModel()
                {
                    RequestId = "Question not found"
                };
                return View("Error", errorViewModel);
            }
            if (question.Type == QuestionType.Range)
            {
                var rangeQuestion = (RangeQuestion) question;
                ViewBag.Min = rangeQuestion.Min;
                ViewBag.Max = rangeQuestion.Max;
            }

            var allQuestionsInFlow = _questionManager.GetQuestionsByFlowId(question.FlowId).ToList();
            ViewBag.FollowUpQuestions = allQuestionsInFlow.Any(q => q.Position > question.Position);
            ViewBag.IsLastQuestion = allQuestionsInFlow.Max(q => q.Position) == question.Position;
            return View(question);
        }

        public async Task<IActionResult> Delete(int questionId)
        {
            _unitOfWork.BeginTransaction();
            var question = _questionManager.GetQuestionById(questionId);

            var questionsToUpdate = _questionManager.GetQuestionsByFlowId(question.FlowId)
                .Where(q => q.Position > question.Position).ToList();

            foreach (var q in questionsToUpdate)
            {
                q.Position--;
            }
            await _questionManager.DeleteAsync(question);
            await _questionManager.UpdateAllAsync(questionsToUpdate);
            
            _optionManager.UpdateOptionsAfterQuestionDeletion(questionId);
            _questionManager.RemoveAnswersByQuestionId(questionId);
            
            _unitOfWork.Commit();
            return RedirectToAction("Edit", "Flow", new {parentFlowId = question.FlowId});
        }

        private Question CreateQuestion(QuestionType type, string text, Media media, int flowId)
        {
            //Search the highest position with this flow id
            var questions = _questionManager.GetQuestionsByFlowId(flowId).ToList();
            int newPosition;
            if (!questions.Any())
            {
                newPosition = 1;
            }
            else
            {
                newPosition = questions.Max(q => q.Position) + 1;
            }
            switch (type)
            {
                case QuestionType.MultipleChoice:
                    return new MultipleChoiceQuestion { Text = text, Type = type, Media = media, FlowId = flowId, Position = newPosition};
                case QuestionType.Open:
                    return new OpenQuestion { Text = text, Type = type, Media = media, FlowId = flowId, Position = newPosition};
                case QuestionType.Range:
                    return new RangeQuestion { Text = text, Type = type, Media = media, FlowId = flowId, Position = newPosition};
                case QuestionType.SingleChoice:
                    return new SingleChoiceQuestion { Text = text, Type = type, Media = media, FlowId = flowId, Position = newPosition};
                default:
                    return new Question { Text = text, Type = type, Media = media, FlowId = flowId, Position = newPosition};
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create(string text, QuestionType type, Media media, int flowId)
        {
            _unitOfWork.BeginTransaction();
            var question = CreateQuestion(type, text, media, flowId);
            await _questionManager.AddAsync(question);
            
            _unitOfWork.Commit();
            return RedirectToAction("Edit", new { questionId = question.Id });
         }

        [HttpGet]
        public IActionResult GetRangeValues(int id)
        {
            var values = _questionManager.GetRangeQuestionValues(id);
            return Json(values);
        }

        [HttpGet]
        public IActionResult StartPreview(int id)
        {
            var question = _questionManager.GetQuestionById(id);
            if (question == null)
            {
                var errorViewModel = new ErrorViewModel()
                {
                    RequestId = "Question not found"
                };
                return View("Error", errorViewModel);
            }

            var questions = _questionManager.GetQuestionsByFlowId(question.FlowId).ToList();
            //get the position of this question in the list
            var currentIndex = questions.IndexOf(question);
            
            return RedirectToAction("RedirectTroughPreview", "Flow", new {redirectedQuestionId = currentIndex, flowId = question.FlowId});
        }
    }
}
