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
    public class TestController() : ControllerBase
    {

        [HttpGet]
        public string WelcomeMessage()
        {
            var message = "Page test";
            Console.WriteLine(message);
            return message;
        }
        /*         [HttpPost("bucket")]
                static async Task ListBuckets()
                {
                    var response = await s3Client.ListBucketsAsync();

                    foreach (var s3Bucket in response.Buckets)
                    {
                        Console.WriteLine("{0}", s3Bucket.BucketName);
                    }
                } */
    }
}