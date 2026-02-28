

namespace Portfolio.Models;

public class SendContactInfoDTO
{
  public string? Email { get; set; }
  public string? Subject { get; set; }
  public string? Content { get; set; }
  public IFormFile? File { get; set; }
}