using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudflare.NET.R2;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Services;

namespace BACK_END.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController(IR2Client r2) : ControllerBase
    {

        [HttpGet]
        public string WelcomeMessage()
        {
            var message = "Page test";
            Console.WriteLine(message);
            return message;
        }
        [HttpPost("bucket")]
        public async Task ListBuckets()
        {
            var result = await r2.ListObjectsAsync(
                bucketName: "my-bucket",
                prefix: null); // null for all objects

            foreach (var obj in result.Data)
            {
                Console.WriteLine($"Key: {obj.Key}");
                Console.WriteLine($"Size: {obj.Size} bytes");
                Console.WriteLine($"Modified: {obj.LastModified}");
                Console.WriteLine($"ETag: {obj.ETag}");
                Console.WriteLine();
            }

        }
    }
}
