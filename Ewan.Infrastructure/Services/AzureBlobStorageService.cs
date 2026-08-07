using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Ewan.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Ewan.Infrastructure.Services
{
    public class AzureBlobStorageService : IFileStorageService
    {
        private readonly BlobContainerClient _containerClient;
        private readonly string _cdnBaseUrl;

        public AzureBlobStorageService(IConfiguration configuration)
        {
            var connectionString = configuration["AzureStorage:ConnectionString"]
                ?? throw new InvalidOperationException("AzureStorage:ConnectionString مش موجود في الإعدادات");

            var containerName = configuration["AzureStorage:ContainerName"] ?? "media";

            var blobServiceClient = new BlobServiceClient(connectionString);
            _containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // بيتنشئ تلقائي أول مرة لو مش موجود، وبيتحط Public Access على مستوى الـ Blob بس (مش الـ Container كله)
            _containerClient.CreateIfNotExists(PublicAccessType.Blob);

            // لو حاطط CDN قدام الـ Storage (مستحسن للإنتاج)، حطه في الإعدادات وهيتستخدم بدل رابط الـ Blob المباشر
            _cdnBaseUrl = configuration["AzureStorage:CdnBaseUrl"] ?? string.Empty;
        }

        public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, string folder)
        {
            var extension = Path.GetExtension(fileName);
            var uniqueName = $"{folder}/{Guid.NewGuid()}{extension}";

            var blobClient = _containerClient.GetBlobClient(uniqueName);

            await blobClient.UploadAsync(fileStream, new BlobHttpHeaders
            {
                ContentType = contentType,
                // الكاش سنة كاملة - الاسم فريد أصلا (GUID) فمفيش مشكلة لو الملف اتغير، هيبقى اسم مختلف
                CacheControl = "public, max-age=31536000"
            });

            return string.IsNullOrEmpty(_cdnBaseUrl)
                ? blobClient.Uri.ToString()
                : $"{_cdnBaseUrl.TrimEnd('/')}/{uniqueName}";
        }

        public async Task<bool> DeleteAsync(string fileUrl)
        {
            // بنستخرج اسم الـ Blob من الرابط الكامل (سواء كان رابط Blob مباشر أو رابط CDN)
            var uri = new Uri(fileUrl);
            var blobName = string.Join("/", uri.Segments.Skip(2)).TrimEnd('/');

            var blobClient = _containerClient.GetBlobClient(blobName);
            var response = await blobClient.DeleteIfExistsAsync();
            return response.Value;
        }
    }
}
