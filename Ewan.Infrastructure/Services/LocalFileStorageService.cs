using Ewan.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Ewan.Infrastructure.Services
{
    // بديل مؤقت لحد ما يتعمل Azure Storage Account - بيحفظ الصور جوه wwwroot/uploads
    // نفس الـ Interface بالظبط، فلو رجعنا لـ Azure بعدين هنبدل سطر واحد بس في DI
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly string _basePath;
        private readonly string _baseUrl;

        public LocalFileStorageService(IConfiguration configuration)
        {
            _basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            Directory.CreateDirectory(_basePath);

            // لازم يتحط في appsettings.json: "PublicBaseUrl": "http://localhost:8080" (أو رابط Railway بعد النشر)
            _baseUrl = configuration["PublicBaseUrl"]?.TrimEnd('/') ?? "";
        }

        public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, string folder)
        {
            var extension = Path.GetExtension(fileName);
            var uniqueName = $"{Guid.NewGuid()}{extension}";

            var folderPath = Path.Combine(_basePath, folder);
            Directory.CreateDirectory(folderPath);

            var fullPath = Path.Combine(folderPath, uniqueName);
            await using var output = File.Create(fullPath);
            await fileStream.CopyToAsync(output);

            return $"{_baseUrl}/uploads/{folder}/{uniqueName}";
        }

        public Task<bool> DeleteAsync(string fileUrl)
        {
            var relativePath = fileUrl.Split("/uploads/").LastOrDefault();
            if (string.IsNullOrEmpty(relativePath)) return Task.FromResult(false);

            var fullPath = Path.Combine(_basePath, relativePath.Replace('/', Path.DirectorySeparatorChar));
            if (!File.Exists(fullPath)) return Task.FromResult(false);

            File.Delete(fullPath);
            return Task.FromResult(true);
        }
    }
}