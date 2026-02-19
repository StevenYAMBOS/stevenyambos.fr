
using Cloudflare.NET.R2;
using Portfolio.Repositories;

namespace Portfolio.Services;

public class FileService(IR2Client r2) : IFileService
{
  public async Task<string> UploadFileAsync(IFormFile file, string[] allowedExtensions, string folder, Guid resourceId)
  {
    ArgumentNullException.ThrowIfNull(file);

    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
    if (!allowedExtensions.Contains(ext))
    {
      throw new ArgumentException($"Extensions autorisées : {string.Join(", ", allowedExtensions)}");
    }

    var objectKey = $"{folder}/{resourceId}{ext}";

    await using var stream = file.OpenReadStream();
    await r2.UploadAsync(
        bucketName: "portfolio-bucket",
        objectKey: objectKey,
        fileStream: stream);

    var R2publicUrl = $"https://pub-56d2c024e16e477e9fe29e4b168d78ec.r2.dev/{objectKey}";

    return R2publicUrl;
  }

  public async Task DeleteFileAsync(string objectKey)
  {
    if (string.IsNullOrEmpty(objectKey))
    {
      throw new ArgumentNullException(nameof(objectKey));
    }

    await r2.DeleteObjectAsync("portfolio-bucket", objectKey);
  }
}