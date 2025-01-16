using BL.Interfaces;
using DAL.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BL.Implementations;

public class CloudManager : ICloudManager
{
    private readonly ICloudStorageRepository _storage;
    public CloudManager(ICloudStorageRepository storage)
    {
         _storage = storage;
    }
    public void UploadFile(IFormFile file, string fileName, string folderName)
    {
        _storage.UploadFile(file, fileName, folderName);
    }
    public bool FileExists(string fileName)
    {
        return _storage.FileExists(fileName);
    }
    public string GetFileExtenstion(string fileName)
    {
        return _storage.GetFileExtension(fileName);
    }
    public void DeleteFile(string fileName)
    {
        _storage.DeleteFile(fileName);
    }
}