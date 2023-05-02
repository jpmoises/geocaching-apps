using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace ExLibrisFunctions
{
    internal class Helper
    {
        const string staticFilesFolder = "www";
        static string defaultPage =
            string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DEFAULT_PAGE")) ?
            "index.html" : Environment.GetEnvironmentVariable("DEFAULT_PAGE");

        public static string GetScriptPath()
            => Path.Combine(GetEnvironmentVariable("HOME"), @"");
            //=> Path.Combine(GetEnvironmentVariable("HOME"), @"site\wwwroot");

        public static string GetEnvironmentVariable(string name)
            => System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);

        public static string GetFilePath(string pathValue, ILogger log, string functionName)
        {
            var path = pathValue ?? "";

            var staticFilesPath =
                Path.GetFullPath(Path.Combine(GetScriptPath(), staticFilesFolder, functionName));
            var fullPath = Path.GetFullPath(Path.Combine(staticFilesPath, path));

            if (!IsInDirectory(staticFilesPath, fullPath))
            {
                throw new ArgumentException("Invalid path");
            }

            var isDirectory = Directory.Exists(fullPath);
            if (isDirectory)
            {
                fullPath = Path.Combine(fullPath, defaultPage);
            }

            return fullPath;
        }

        public static bool IsInDirectory(string parentPath, string childPath)
        {
            var parent = new DirectoryInfo(parentPath);
            var child = new DirectoryInfo(childPath);

            var dir = child;
            do
            {
                if (dir.FullName == parent.FullName)
                {
                    return true;
                }
                dir = dir.Parent;
            } while (dir != null);

            return false;
        }

        public static bool IsMobileDevice(HttpRequest r)
        {
            String userAgent = r.Headers["User-Agent"];
            String deviceName = "Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini";
            return Regex.IsMatch(userAgent, deviceName);
        }
    }
}
