

namespace Portfolio.Repositories;

public interface IFileService
{
  Task<string> SaveFileAsync(IFormFile imageFile, string[] allowedFileExtensions, string path, Guid articleId);
  void DeleteFile(string fileNameWithExtension);
}