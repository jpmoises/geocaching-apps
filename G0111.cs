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
    public static class G0111
    {
        const string staticFilesFolder = "www";
        static string defaultPage =
            string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DEFAULT_PAGE")) ?
            "index.html" : Environment.GetEnvironmentVariable("DEFAULT_PAGE");
        const int max = 6;
        [FunctionName("G0111")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "g0111/{file?}")] HttpRequest req,
            ILogger log, string file)
        {

			var result = "";
            var numguess = 0;
			string coords = "N 29 36.409 W 082 25.983";

			if (req.Method.Equals("GET"))
            {
				Random r = new Random();
				int rInt = r.Next(0, 100);
				try
				{
					var filePath = Helper.GetFilePath(file, log, "render");

					var response = new HttpResponseMessage(HttpStatusCode.OK);
					var stream = new FileStream(filePath, FileMode.Open);

					string fileContents;
					using (StreamReader reader = new StreamReader(stream))
					{
						fileContents = reader.ReadToEnd();
					}

                    response.Content = new StringContent(fileContents.Replace("<guesscnt>", max.ToString()).Replace("<answer>", rInt.ToString()).Replace("<status>", "").Replace("<disabled>", ""));
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
			}
            else
            {
				if (req.ContentLength == 0)
				{
					return new HttpResponseMessage(HttpStatusCode.BadRequest);
				}
				int guess = Int32.Parse(req.ReadFormAsync().Result["guess"]);
                int answer = Int32.Parse(req.ReadFormAsync().Result["answer"]);
                Int32.TryParse(req.ReadFormAsync().Result["numtry"], out int numtry);
                numtry++;
                string status = "";
                string disabled = "";

                if (max - numtry == 0)
                {
                    status = "<p>Whomp, whomp, try again</p>";
                    disabled = "disabled";
                }
                else if (guess > answer)
                {
                    status = "<p>Nope, go lower</p>";
                }
                else if (guess < answer)
                {
                    status = "<p>Nope, go higher</p>";
                }
                else
                {
                    status = $"<p>Wow, you guessed {answer} correctly! The final coordinates are: {coords} </p>";

					disabled = "disabled";
				}
                    try
                    {
                        var filePath = Helper.GetFilePath(file, log, "render");

                        var response = new HttpResponseMessage(HttpStatusCode.OK);
                        var stream = new FileStream(filePath, FileMode.Open);

                        string fileContents;
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            fileContents = reader.ReadToEnd();
                        }

                        response.Content = new StringContent(fileContents.Replace("<guesscnt>", (max - numtry).ToString()).Replace("<answer>", answer.ToString()).Replace("<numtry>", numtry.ToString()).Replace("<status>", status).Replace("<disabled>", disabled));
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

			}

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
