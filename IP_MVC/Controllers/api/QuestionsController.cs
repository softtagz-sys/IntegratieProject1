using System.Text.Json;
using System.Web;
using BL.Domain;
using BL.Domain.Questions;
using BL.Implementations;
using BL.Interfaces;
using Google.Cloud.Storage.V1;
using IP_MVC.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace IP_MVC.Controllers.api
{
    [ApiController]
    [Route("/api/[controller]")]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionManager _questionManager;
        private readonly UnitOfWork _unitOfWork;

        public QuestionsController(IQuestionManager questionManager, UnitOfWork unitOfWork)
        {
            _questionManager = questionManager;
            _unitOfWork = unitOfWork;
        }

        [HttpPut("{id}/Title")]
        public IActionResult UpdateTitle(int id, [FromBody] string text)
        {
            _unitOfWork.BeginTransaction();
            var question = _questionManager.GetQuestionById(id);

            if (question == null)
            {
                return NotFound();
            }

            var newQuestion = question;
            newQuestion.Text = text;

            _questionManager.UpdateAsync(question, newQuestion);

            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("UpdateRangeQuestion")]
        public IActionResult UpdateRangeQuestion([FromQuery] int id, [FromQuery] int min, [FromQuery] int max)
        {
            _unitOfWork.BeginTransaction();
            var question = _questionManager.GetQuestionById(id);
            if (question == null)
            {
                return NotFound();
            }

            _questionManager.SetRangeQuestionValues(id, min, max);

            _unitOfWork.Commit();
            return Ok();
        }

        [HttpPost("UploadMedia")]
        public async Task<IActionResult> UploadMedia([FromForm] IFormFile file, [FromForm] int questionId)
        {
            try
            {
                _unitOfWork.BeginTransaction();
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file was uploaded.");
                }

                var filePath = Path.GetTempFileName();

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Upload the file to Google Cloud Storage
                var storage = StorageClient.Create();
                using var fileStream = System.IO.File.OpenRead(filePath);
                var fileName = HttpUtility.UrlEncode(file.FileName);
                var objectName = $"Questions/{fileName}";
                string json = await System.IO.File.ReadAllTextAsync("service-acc-key.json");
                JsonDocument doc = JsonDocument.Parse(json);
                string projectId = doc.RootElement.GetProperty("project_id").GetString();
                storage.UploadObject(projectId + "-public", objectName, null, fileStream);
                Console.WriteLine($"Uploaded {objectName}.");
                //encode the file name to avoid any special characters

                // Save the URL of the uploaded file in your database
                var fileUrl = $"https://storage.googleapis.com/{projectId}-public/{objectName}";
                //encode the file name to avoid any special characters


                var description = "Uploaded media";
                // get the media type from the file extension
                var mediaType = file.ContentType switch
                {
                    "video/mp4" => MediaType.VIDEO,
                    "image/jpeg" => MediaType.IMAGE,
                    "image/png" => MediaType.IMAGE,
                    "audio/mpeg" => MediaType.AUDIO,
                    _ => throw new Exception("Unsupported media type")
                };
                _questionManager.AddMediaToQuestion(questionId, fileUrl, description, mediaType);

                _unitOfWork.Commit();
                return Ok(new { filePath = fileUrl });
            }
            catch (Exception ex)
            {
                // Log the exception message and stack trace
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);

                // Return a 500 status code with a detailed error message
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("{questionId}/Reorder/{newPosition}")]
        public IActionResult Reorder(int questionId, int newPosition)
        {
            _unitOfWork.BeginTransaction();
            var question = _questionManager.GetQuestionById(questionId);
            var oldPosition = question.Position;
            question.Position = newPosition;

            List<Question> affectedQuestions;
            if (newPosition < oldPosition)
            {
                // The question has been moved up, so increment the position of all questions between the old and new position
                affectedQuestions = _questionManager
                    .GetQuestionsBetweenPositionsByFlowId(question.FlowId, newPosition, oldPosition - 1).ToList();
                foreach (var affectedQuestion in affectedQuestions)
                {
                    affectedQuestion.Position++;
                }
            }
            else
            {
                // The question has been moved down, so decrement the position of all questions between the old and new position
                affectedQuestions = _questionManager
                    .GetQuestionsBetweenPositionsByFlowId(question.FlowId, oldPosition + 1, newPosition).ToList();
                foreach (var affectedQuestion in affectedQuestions)
                {
                    affectedQuestion.Position--;
                }
            }

            // Add the initially moved question to the list of affected questions
            affectedQuestions.Add(question);

            // Update all affected questions at once
            _questionManager.UpdateAllAsync(affectedQuestions);

            _unitOfWork.Commit();
            return RedirectToAction("Edit", "Flow", new { parentFlowId = question.FlowId });
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] QuestionCreateDto createDto)
        {
            _unitOfWork.BeginTransaction();
            if (createDto == null)
            {
                return BadRequest("Invalid flow data.");
            }

            Question newQuestion;

            switch (createDto.Type)
            {
                case "MultipleChoice":
                    newQuestion = new MultipleChoiceQuestion();
                    newQuestion.Type = QuestionType.MultipleChoice;
                    break;
                case "SingleChoice":
                    newQuestion = new SingleChoiceQuestion();
                    newQuestion.Type = QuestionType.SingleChoice;
                    break;
                case "Open":
                    newQuestion = new OpenQuestion();
                    newQuestion.Type = QuestionType.Open;
                    break;
                case "Range":
                    newQuestion = new RangeQuestion();
                    newQuestion.Type = QuestionType.Range;
                    break;
                default:
                    return BadRequest("Invalid question type.");
            }

            newQuestion.Text = createDto.Text;
            newQuestion.FlowId = createDto.FlowId;

            var listQuestionsByFlowId = _questionManager.GetQuestionsByFlowId(createDto.FlowId).ToList();
            var newPosition = 1;
            if (listQuestionsByFlowId.Any())
            { 
                newPosition = listQuestionsByFlowId.Max(q => q.Position) + 1;
            }
            newQuestion.Position = newPosition;
            
            await _questionManager.AddAsync(newQuestion);

            _unitOfWork.Commit();

            return Ok(newQuestion);
        }
        
        [HttpGet("RedirectableQuestions")]
        public IActionResult RedirectableQuestions([FromQuery] int flowId, [FromQuery] int position)
        {
            var questions = _questionManager.GetQuestionsByFlowIdAfterPosition(flowId, position);
            if (questions == null || !questions.Any())
            {
                return NoContent();
            }
            return Ok(questions);
        }
    }
}