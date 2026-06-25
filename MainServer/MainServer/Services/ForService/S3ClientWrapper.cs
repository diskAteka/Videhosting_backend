using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;

namespace MainServer.Services.ForService
{

    public class S3ClientWrapper
    {
        private readonly IAmazonS3 _client;
        private readonly string _bucketName;
        private readonly ILogger<S3ClientWrapper> _logger;

        public S3ClientWrapper(IOptions<GarageOptions> options, ILogger<S3ClientWrapper> logger)
        {
            _logger = logger;

            var opts = options.Value;
            _bucketName = opts.BucketName;

            var config = new AmazonS3Config
            {
                ServiceURL = opts.Endpoint,
                ForcePathStyle = true,
                AuthenticationRegion = "garage"
            };

            _client = new AmazonS3Client(opts.AccessKey, opts.SecretKey, config);
        }

        public async Task EnsureBucketExistsAsync()
        {
            try
            {
                await _client.PutBucketAsync(_bucketName);
                _logger.LogInformation("Бакет {Bucket} создан", _bucketName);
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.Conflict) { }
        }

        public async Task UploadFileAsync(string filePath, string objectKey, string contentType)
        {
            await EnsureBucketExistsAsync();

            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = objectKey,
                FilePath = filePath,
                ContentType = contentType
            };

            await _client.PutObjectAsync(request);
            _logger.LogInformation("Загружен объект {Key}", objectKey);
        }

        public async Task<GetObjectResponse> GetObjectAsync(string objectKey)
        {
            var request = new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = objectKey
            };

            return await _client.GetObjectAsync(request);
        }

        public async Task DeleteObjectsAsync(params string[] keys)
        {
            if (keys.Length == 0) return;

            var request = new DeleteObjectsRequest
            {
                BucketName = _bucketName,
                Objects = keys.Select(k => new KeyVersion { Key = k }).ToList()
            };

            var response = await _client.DeleteObjectsAsync(request);

            if (response.DeleteErrors.Any())
            {
                var errors = string.Join(", ", response.DeleteErrors.Select(e => $"{e.Key}: {e.Message}"));
                _logger.LogWarning("Ошибки удаления объектов: {Errors}", errors);
            }
        }

        public Task<string> GeneratePresignedUrlAsync(string objectKey, int expiryHours)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = objectKey,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.AddHours(expiryHours)
            };

            return Task.FromResult(_client.GetPreSignedURL(request));
        }

        public async Task<bool> IsAvailableAsync()
        {
            try
            {
                await _client.ListBucketsAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "S3-хранилище недоступно");
                return false;
            }
        }
    }
}
