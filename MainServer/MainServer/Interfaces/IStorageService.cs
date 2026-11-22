using Amazon.S3.Model;
using MainServer.Services.ForService;

namespace MainServer.Interfaces
{
    public interface IStorageService
    {
        /// <summary>
        /// Загружает видео в хранилище. Постер берётся из <paramref name="tempPosterPath"/>,
        /// а если он не передан — извлекается кадр из видео.
        /// </summary>
        Task<StorageUploadResult> UploadToStorageAsync(string tempVideoPath, string? tempPosterPath = null);
        Task<GetObjectResponse> GetObjectAsync(string bucketName, string key);
        Task<bool> DeleteVideoFilesAsync(string videoKey, string posterKey);
        Task<string> GeneratePresignedUrlAsync(string objectKey, int expiryHours = 1);
        Task<bool> CheckStorageAvailabilityAsync();
    }
}
