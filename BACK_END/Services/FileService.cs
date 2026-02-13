
using Portfolio.Repositories;

public class FileService(IWebHostEnvironment environment) : IFileService
{
  public void DeleteFile(string fileNameWithExtension)
  {
    throw new NotImplementedException();
  }

  public async Task<string> SaveFileAsync(IFormFile imageFile, string[] allowedFileExtensions, string path, Guid articleId)
  {
    if (imageFile == null)
    {
      throw new ArgumentNullException(nameof(imageFile));
    }
    var ext = Path.GetExtension(imageFile.FileName);
    if (!allowedFileExtensions.Contains(ext))
    {
      throw new ArgumentException($"Seules les extentions suivantes sont autorisées : {string.Join(",", allowedFileExtensions)}");
    }

    var fileName = $"{articleId.ToString()}{ext}";
    var fileNameWithPath = Path.Combine(path, fileName);
    using var stream = new FileStream(fileNameWithPath, FileMode.Create);
    await imageFile.CopyToAsync(stream);
    return fileName;
  }
}