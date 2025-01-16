using Microsoft.AspNetCore.Http;

namespace BL.Interfaces;

public interface ICloudManager
{
    void UploadFile(IFormFile file, string fileName, string folderName);
    bool FileExists(string fileName);
    string GetFileExtenstion(string fileName);
    void DeleteFile(string fileName);
}