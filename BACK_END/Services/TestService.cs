using Portfolio.Data;
using Portfolio.Entities;
using Portfolio.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Cloudflare.NET.R2;

namespace Portfolio.Services
{

  public interface ITestService
  {
    Task ListFilesAsync();
    Task ListFilesAsync(string prefix);
  }
  public class TestService(IR2Client r2)
  {
    public async Task ListFilesAsync(string prefix)
    {
      var result = await r2.ListObjectsAsync(
          bucketName: "portfolio-bucket",
          prefix: null);

      foreach (var obj in result.Data)
      {
        Console.WriteLine($"Key: {obj.Key}");
        Console.WriteLine($"Size: {obj.Size} bytes");
        Console.WriteLine($"Modified: {obj.LastModified}");
        Console.WriteLine($"ETag: {obj.ETag}");
        Console.WriteLine();
      }


      Console.WriteLine($"Class A operations: {result.Metrics.ClassAOperations}");
    }
  }
}