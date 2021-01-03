using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using VideoUploader_ConfettiService.Data;

namespace VideoUploader_ConfettiService
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {

        private readonly GoogleCredential _googleCredential;
        private readonly StorageClient _storageClient;
        private readonly string _bucketName;
        private readonly IHostingEnvironment _hostEnv;
        private VideoDbContext _videoDbContext;
        public VideoController(IConfiguration config,VideoDbContext videoDb, IHostingEnvironment hostingEnvrionment)
        {
            _googleCredential = GoogleCredential.FromFile(config["GoogleCloudFile"]); // get credentials using filename from appsettings
            _storageClient = StorageClient.Create(_googleCredential);
            _bucketName = config["GCPBucketName"]; // get bucketname from appsettings
            _videoDbContext = videoDb;
            _hostEnv = hostingEnvrionment;

        }


        private string GenerateBucketURL(Guid id, string filename)
        {
            return filename.Insert(filename.LastIndexOf("."), id.ToString()); // this will find the extension and include the id of the video so we can identify it using the original filename+id, that way if we get two files with the same named uploaded 
        }


      
        [HttpPost]
        [RequestSizeLimit(1073741824)]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]

        public async Task<IActionResult> UploadVideo([FromForm] IFormFile file, [FromForm] string Title, [FromForm] string Username)
        {

            if (String.IsNullOrWhiteSpace(Title) || String.IsNullOrWhiteSpace(Username))
                return BadRequest();

            Guid vidId = Guid.NewGuid();

            var safeLinkURL = Microsoft.AspNetCore.WebUtilities.WebEncoders.Base64UrlEncode(vidId.ToByteArray());
            var videoRecord = new Video
            {
                VideoId = vidId,
                Title = Title,
                Username = Username,
                PostedAt = DateTime.UtcNow,
                BucketURL = GenerateBucketURL(vidId,file.FileName),
                LinkURL = safeLinkURL

            };

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    await _storageClient.UploadObjectAsync(_bucketName, videoRecord.BucketURL, null, memoryStream);

                }

            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                return StatusCode(500);
            }


            try
            {
                _videoDbContext.Videos.Add(videoRecord);
                _videoDbContext.SaveChanges();
            }catch(Exception e)
            {

                Console.WriteLine(e.ToString());

                //Video record saved to fail so we need to remove the video file in the GCP Storage so it's not left stranded
                await _storageClient.DeleteObjectAsync(_bucketName, videoRecord.BucketURL);

                return StatusCode(500);
            }


            return Ok(videoRecord);
        }

        /*
         * This function will download the video given the video's URL
         * This function connects to cloud storage services, in this example we're using GCP but can be replace with Azure or AWS
         */
        [HttpGet("{url}")]
        public async Task<IActionResult> DownloadVideo(string url)
        {

            var videoRecordToDownload = _videoDbContext.Videos.SingleOrDefault(v => v.LinkURL == url);

            if (videoRecordToDownload == null)
                return BadRequest("Invalid URL: No video found with the URL: " + url);

            MemoryStream videoStream = new MemoryStream(); // create memory stream to store video data once we retrieve it

            await _storageClient.DownloadObjectAsync(_bucketName, videoRecordToDownload.BucketURL,videoStream);
            videoStream.Position = 0;

            string mimeType="";

            new FileExtensionContentTypeProvider().TryGetContentType(videoRecordToDownload.BucketURL, out mimeType);

            var file = File(videoStream, mimeType, videoRecordToDownload.BucketURL);
           
            return file;
        }
    }
}
