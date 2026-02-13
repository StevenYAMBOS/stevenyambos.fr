

using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Portfolio.Repositories;

public interface IFileService
{
  Task SaveFileAsync(IFormFile imageFile, string[] allowedFileExtensions, string path, Guid articleId);
  void DeleteFile(string fileNameWithExtension);
}