using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using Microsoft.Extensions.Configuration;
using ShyrochenkoPatterns.Services.Interfaces.External;
using System.IO;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Services.External
{
    public class S3Service : IS3Service
    {
        private AmazonS3Client _amazonS3Client;
        private IConfiguration _configuration;

        private string _urlTemplate;
        private string _bucket;
        private string _folder;

        public S3Service(IConfiguration configuration)
        {
            _configuration = configuration;

            var cridentials = new BasicAWSCredentials(configuration["AWS:AccessKey"], configuration["AWS:SecretKey"]);
            _amazonS3Client = new AmazonS3Client(cridentials, Amazon.RegionEndpoint.USEast1);

            _urlTemplate = _configuration["AWS:UrlTemplate"];
            _bucket = _configuration["AWS:Bucket"];
            _folder = _configuration["AWS:Folder"];
        }

        public async Task<string> UploadFile(Stream stream, string key)
        {
            var uploadRequest = new TransferUtilityUploadRequest();
            uploadRequest.CannedACL = S3CannedACL.PublicRead;
            uploadRequest.InputStream = stream;
            uploadRequest.BucketName = _bucket;
            uploadRequest.Key = $"{_folder}/{key}";

            using (TransferUtility fileTransferUtility = new TransferUtility(_amazonS3Client))
            {
                await fileTransferUtility.UploadAsync(uploadRequest);
            }

            return string.Format(_urlTemplate, _bucket, _folder, key);
        }

        public async Task DeleteFile(string name)
        {
            var response = await _amazonS3Client.DeleteObjectAsync(new DeleteObjectRequest
            {
                BucketName = _bucket,
                Key = _folder + "/" + name
            });
        }

        private async Task EnsureBucketCreatedAsync(string bucketName)
        {
            if (!(await AmazonS3Util.DoesS3BucketExistAsync(_amazonS3Client, bucketName)))
            {
                throw new AmazonS3Exception(string.Format("Bucket is missing", bucketName));
            }
        }
    }
}
