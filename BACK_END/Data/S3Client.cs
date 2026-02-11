using Amazon.Runtime;
using Amazon.S3;

namespace Test
{
    class AppBucketContext()
    {
        private static IAmazonS3 s3Client;

        public static void S3Bucket(string[] args)
        {
            // Retrieve your S3 API credentials for your R2 bucket via API tokens (see: https://developers.cloudflare.com/r2/api/tokens)
            var accessKey = "";
            var secretKey = "";
            var credentials = new BasicAWSCredentials(accessKey, secretKey);
            s3Client = new AmazonS3Client(credentials, new AmazonS3Config
            {
                // Provide your Cloudflare account ID
                ServiceURL = "",
            });
        }
    }
}
