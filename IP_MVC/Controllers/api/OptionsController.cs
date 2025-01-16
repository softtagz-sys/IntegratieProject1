using BL.Domain.Questions;
using BL.Implementations;
using BL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IP_MVC.Controllers.api
{
    [ApiController]
    [Route("/api/[controller]")]
    public class OptionsController : ControllerBase
    {
        private readonly IQuestionManager _questionManager;
        private readonly IOptionManager _optionManager;
        private readonly UnitOfWork _unitOfWork;

        public OptionsController(IQuestionManager questionManager, IOptionManager optionManager, UnitOfWork unitOfWork)
        {
            _questionManager = questionManager;
            _optionManager = optionManager;
            _unitOfWork = unitOfWork;
        }
        
        [HttpGet("GetAllOptions/{id}")]
        public IActionResult GetAllOptions(int id)
        {
            var options = _optionManager.GetOptionsSingleOrMultipleChoiceQuestion(id);
            
            if (options == null)
            {
                return NotFound();
            }

            if (!options.Any())
            {
                return Ok(new List<Option>());
            }

            return Ok(options);
        }
        
        [HttpPut("AddOption/{id}")]
        public IActionResult AddOption(int id, [FromBody] string option)
        {
            _unitOfWork.BeginTransaction();
            var question = _questionManager.GetQuestionById(id);
            
            if (question == null)
            {
                return NotFound();
            }

            _optionManager.AddOptionToQuestion(id, option);
            
            _unitOfWork.Commit();
            return NoContent();
        }
        
        [HttpPost("DeleteOption")]
        public IActionResult DeleteOption([FromQuery] int id)
        {
            _unitOfWork.BeginTransaction();

            var option = _optionManager.GetAsync(id).Result;
            _optionManager.DeleteAsync(option);
            
            _unitOfWork.Commit();
            return Ok();
        }
        
        [HttpPost("ChangeRedirectedIdFromOption")]
        public IActionResult ChangeRedirectedIdFromOption([FromQuery] int questionId, [FromQuery] int selectedOption, [FromQuery] int nextQuestionId)
        {
            _unitOfWork.BeginTransaction();
            var question = _questionManager.GetQuestionById(questionId);
            if (question == null)
            {
                return NotFound();
            }

            var option = _optionManager.GetOptionsSingleOrMultipleChoiceQuestion(questionId).FirstOrDefault(o => o.Id == selectedOption);
            if (option == null)
            {
                return NotFound();
            }
            _optionManager.UpdateOption(questionId, option.Id,nextQuestionId);
            
            _unitOfWork.Commit();
            return Ok();
        }

        [HttpGet("GetOptionById/{id}")]
        public IActionResult GetOptionById(int id)
        {
            var option = _optionManager.GetAsync(id).Result;
            if (option == null)
            {
                return NotFound();
            }
            return Ok(option);
        }
    }
}