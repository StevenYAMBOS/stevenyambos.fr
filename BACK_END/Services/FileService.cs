
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

    return objectKey;
  }

  public Task DeleteFileAsync(string objectKey)
  {
    throw new NotImplementedException();
  }
}