using ExLibrisFunctions;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace GeoCaching
{
    public static class Ducky
    {
        const string staticFilesFolder = "www";
        static string defaultPage =
            string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DEFAULT_PAGE")) ?
            "index.html" : Environment.GetEnvironmentVariable("DEFAULT_PAGE");
        
        [FunctionName("ducky")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ducky/{file?}")] HttpRequest req,
            ILogger log, string file)
        {
            if (Helper.IsMobileDevice(req))
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent("The final coords are: N 29° 40.784 W 082° 26.146");
                return response;
            }
            else
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent("Sorry, mobile device was not detected.");
                return response;
            }
            /*try
            {
                var filePath = Helper.GetFilePath(file, log, "adaform");

                var response = new HttpResponseMessage(HttpStatusCode.OK);
                var stream = new FileStream(filePath, FileMode.Open);
                response.Content = new StreamContent(stream);
                response.Content.Headers.ContentType =
                    new MediaTypeHeaderValue(MimeTypes.GetMimeType(filePath));
                return response;
            }
            catch
            {
                var filePath = Helper.GetFilePath(file, log, "adaform");
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(filePath);
                return response;
            }
            */
        }

        //private static string GetScriptPath()
        //    => Path.Combine(GetEnvironmentVariable("HOME"), @"site\wwwroot");

        //private static string GetEnvironmentVariable(string name)
        //    => System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);

        //private static string GetFilePath(string pathValue, ILogger log, string functionName)
        //{
        //    var path = pathValue ?? "";

        //    var staticFilesPath =
        //        Path.GetFullPath(Path.Combine(GetScriptPath(), staticFilesFolder, functionName));
        //    var fullPath = Path.GetFullPath(Path.Combine(staticFilesPath, path));

        //    if (!IsInDirectory(staticFilesPath, fullPath))
        //    {
        //        throw new ArgumentException("Invalid path");
        //    }

        //    var isDirectory = Directory.Exists(fullPath);
        //    if (isDirectory)
        //    {
        //        fullPath = Path.Combine(fullPath, defaultPage);
        //    }

        //    return fullPath;
        //}

        //private static bool IsInDirectory(string parentPath, string childPath)
        //{
        //    var parent = new DirectoryInfo(parentPath);
        //    var child = new DirectoryInfo(childPath);

        //    var dir = child;
        //    do
        //    {
        //        if (dir.FullName == parent.FullName)
        //        {
        //            return true;
        //        }
        //        dir = dir.Parent;
        //    } while (dir != null);

        //    return false;
        //}
    }
}
