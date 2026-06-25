using Amazon.S3.Model;
using SharedLib.Contracts.Request.Video;
using SharedLib.Models;

namespace MainServer.Interfaces
{
    public interface IStorageService
    {
        Task<Video> UploadToStorageAsync(VideoUploadRequest request, int AuthorId, string FileName);
        Task<GetObjectResponse> GetObjectAsync(string bucketName, string url);
        Task<bool> DeleteVideoFilesAsync(string videoKey, string posterKey);
        Task<string> GeneratePresignedUrlAsync(string objectKey, int expiryHours = 1);
        Task<bool> CheckStorageAvailabilityAsync();
    }
}
