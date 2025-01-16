using BL.Implementations;
using BL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IP_MVC.Controllers;

public class UploadController : Controller
{
    private readonly ICloudManager _cloudManager;
    private readonly UnitOfWork _unitOfWork;

    public UploadController(ICloudManager cloudManager, UnitOfWork unitOfWork)
    {
        _cloudManager = cloudManager;
        _unitOfWork = unitOfWork;
    }

    [HttpPost]
    public IActionResult UploadFile(IFormFile file, string fileName, string folderName)
    {
        _unitOfWork.BeginTransaction();
        _cloudManager.UploadFile(file, fileName, folderName);
        
        _unitOfWork.Commit();
        return Ok();
        //TODO: Return to flows page
    }
}