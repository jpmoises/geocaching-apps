using ExLibrisFunctions;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GeoCaching
{
    public static class Render
    {
        const string staticFilesFolder = "www";
        const string path = "render";
        static string defaultPage =
            string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DEFAULT_PAGE")) ?
            "index.html" : Environment.GetEnvironmentVariable("DEFAULT_PAGE");
        
        [FunctionName("render")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = $"{path}/{{file?}}")] HttpRequest req,
            ILogger log, string file)
        {
            try
            {
                var filePath = Helper.GetFilePath(file, log, path);

                var response = new HttpResponseMessage(HttpStatusCode.OK);
                var stream = new FileStream(filePath, FileMode.Open);
                response.Content = new StreamContent(stream);
                response.Content.Headers.ContentType =
                    new MediaTypeHeaderValue(MimeTypes.GetMimeType(filePath));
                return response;
            }
            catch
            {
                var filePath = Helper.GetFilePath(file, log, path);
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(filePath);
                return response;
            }
        }
    }
}
