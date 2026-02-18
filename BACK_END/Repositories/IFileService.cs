

using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Portfolio.Repositories;

public interface IFileService
{
  Task<string> UploadFileAsync(IFormFile file, string[] allowedExtensions, string folder, Guid resourceId);
  Task DeleteFileAsync(string objectKey);
}