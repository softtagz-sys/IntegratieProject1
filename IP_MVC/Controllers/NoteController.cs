using BL.Domain;
using BL.Implementations;
using BL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IP_MVC.Controllers;

public class NoteController(
    IQuestionManager questionManager,
    ISessionManager sessionManager,
    INoteManager noteManager,
    UnitOfWork unitOfWork) : Controller
{
    
    [HttpPost]
    public IActionResult CreateNote(int questionId, string content)
    {
        unitOfWork.BeginTransaction();
        int sessionId = HttpContext.Session.GetInt32("sessionId") ?? 0;
        // Convert the questionId and sessionId from string to int
        if (questionId == 0 || sessionId == 0)
        {
            return BadRequest("Invalid questionId or sessionId");
        }

        // Check if the questionId and sessionId exist in the database
        var question = questionManager.GetQuestionById(questionId);
        var session = sessionManager.GetSessionById(sessionId);
        if (question == null || session == null)
        {
            return NotFound("Question or Session not found");
        }

        // Create a new Note object with the provided note string
        var newNote = new Note { Content = content, QuestionId = questionId, SessionId = sessionId };
        
        //Save the changes to the database
        noteManager.AddAsync(newNote);
        
        unitOfWork.Commit();
        // return Ok(newNote);
        return NoContent();
    }
}