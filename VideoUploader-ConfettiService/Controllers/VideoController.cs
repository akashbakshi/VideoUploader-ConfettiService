using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace VideoUploader_ConfettiService
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {

        private readonly GoogleCredential _googleCredential;
        private readonly StorageClient _storageClient;
        private readonly string _bucketName;
        public VideoController(IConfiguration config)
        {
            _googleCredential = GoogleCredential.FromFile(config["GoogleCloudFile"]);
            _storageClient = StorageClient.Create(_googleCredential);
            _bucketName = config["GCPBucketName"];
        }

        [HttpPost]
        public async Task<IActionResult> UploadVideo(IFormFile file)
        {
            using(var memoryStream = new MemoryStream())
            {
               await file.CopyToAsync(memoryStream);
               await _storageClient.UploadObjectAsync(_bucketName,file.FileName,null,memoryStream);

            }
            return Ok();
        }

        /*
         * This function will download the video given the video's URL
         * This function connects to cloud storage services, in this example we're using GCP but can be replace with Azure or AWS
         */
        [HttpGet("{url}")]
        public async Task<IActionResult> DownloadVideo(string url)
        {
            string objectName = "sample-mp4-file.mp4"; // hardcoded file for testing purposes

            MemoryStream videoStream = new MemoryStream(); // create memory stream to store video data once we retrieve it

            await _storageClient.DownloadObjectAsync(_bucketName, objectName,videoStream);
            videoStream.Position = 0;

            string mimeType="";

            new FileExtensionContentTypeProvider().TryGetContentType(objectName, out mimeType);

            var file = File(videoStream, mimeType, objectName);
           
            return file;
        }
    }
}
