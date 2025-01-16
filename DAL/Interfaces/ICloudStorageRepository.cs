using Microsoft.AspNetCore.Http;

namespace DAL.Interfaces;

public interface ICloudStorageRepository
{
    void UploadFile(IFormFile file, string fileName,string folderName);
    bool FileExists(string fileName);
    string GetFileExtension(string fileName);
    void DeleteFile(string fileName);
}