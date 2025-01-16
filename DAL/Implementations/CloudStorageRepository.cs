using System.Text.Json;
using DAL.Interfaces;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;

namespace DAL.Implementations;

public class CloudStorageRepository : ICloudStorageRepository
{
    private readonly string _bucketName;

    public CloudStorageRepository()
    {
        _bucketName= GetProjectId() + "-public";
        //set the environment variable to the path of the service account key
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "service-acc-key.json");
    }

    private async Task<string> GetProjectId()
    {
        string json = await System.IO.File.ReadAllTextAsync("service-acc-key.json");
        JsonDocument doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("project_id").GetString();
    }
    public void UploadFile(IFormFile file, string fileName, string folderName)
    {
        var storage = StorageClient.Create();
        // Create a memory stream to store the uploaded file
        using var memoryStream = new MemoryStream();
        file.CopyTo(memoryStream);
        // Reset the position of the memory stream to 0
        memoryStream.Position = 0;

        // Get the file extension and content type
        var fileExtension = Path.GetExtension(file.FileName)?.ToLower();
        var contentType = fileExtension switch
        {
            ".jpg" => "image/jpeg",
            ".png" => "image/png",
            _ => "application/octet-stream" // default content type fallback
        };

        // Upload the file to the specified folder in the bucket
        storage.UploadObject(_bucketName, $"{folderName}/{fileName}{fileExtension}", contentType, memoryStream);
    }
    public bool FileExists(string fileName)
    {
        // List all objects in the bucket
        var storage = StorageClient.Create();
        // Check if the file exists in the bucket
        foreach (var obj in storage.ListObjects(_bucketName, ""))
        {
            if (obj.Name.Contains(fileName))
            {
                return true;
            }
        }
        return false;
    }
    public string GetFileExtension(string fileName)
    {
        var storage = StorageClient.Create();
        foreach (var obj in storage.ListObjects(_bucketName, ""))
        {
            if (obj.Name.Contains(fileName))
            {
                // Return the file extension of the object
                return Path.GetExtension(obj.Name);
            }
        }
        return null;
    }
    public void DeleteFile(string fileName)
    {
        var storage = StorageClient.Create();
        string fullFileName = storage.ListObjects(_bucketName, "").FirstOrDefault(obj => obj.Name.Contains(fileName))?.Name;
        // Delete the file from the bucket
        storage.DeleteObject(_bucketName, fullFileName);
    }
}