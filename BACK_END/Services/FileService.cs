
using Cloudflare.NET.R2;
using Portfolio.Repositories;

namespace Portfolio.Services;

public class FileService(IR2Client r2) : IFileService
{
  public void DeleteFile(string fileNameWithExtension)
  {
    throw new NotImplementedException();
  }

  public async Task SaveFileAsync(IFormFile imageFile, string[] allowedFileExtensions, string path, Guid articleId)
  {
    ArgumentNullException.ThrowIfNull(imageFile);
    var ext = Path.GetExtension(imageFile.FileName);
    if (!allowedFileExtensions.Contains(ext))
    {
      throw new ArgumentException($"Seules les extentions suivantes sont autorisées : {string.Join(",", allowedFileExtensions)}");
    }

    var fileName = $"{articleId}.{ext}";
    var fileNameWithPath = Path.Combine(path, fileName);
    // using var stream = new FileStream(fileNameWithPath, FileMode.Create);
    // await imageFile.CopyToAsync(stream);
    // return fileName;
    await using var stream = File.OpenRead(fileNameWithPath);
    var result = await r2.UploadAsync(
    bucketName: "portfolio-bucket",
    objectKey: fileNameWithPath,
    fileStream: stream);

    // return result;
  }
}