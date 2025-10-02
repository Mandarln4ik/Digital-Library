using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Digital_Library.Services
{
    internal class DocumentDownloader
    {
        private static string Server = "https://mandarinka.my-vm.work/digital_library";
        private string defaulCacheLocation;
        string errorImg = "pack://application:,,,/icons/no-image.jpeg";

        public DocumentDownloader()
        {
            defaulCacheLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Cache");

            if (!Path.Exists(defaulCacheLocation))
            {
                Directory.CreateDirectory(defaulCacheLocation);
            }
            if (!Path.Exists(Path.Combine(defaulCacheLocation, "covers")))
            {
                Directory.CreateDirectory(Path.Combine(defaulCacheLocation, "covers"));
            }

            if (!Path.Exists(Path.Combine(defaulCacheLocation, "books")))
            {
                Directory.CreateDirectory(Path.Combine(defaulCacheLocation, "books"));
            }
        }

        public async Task<string> DownloadCover(string path)
        {
            if (String.IsNullOrEmpty(path)) { return errorImg; }
            

            string spath = path.Replace('/', '\\');
            string filePath = $"{defaulCacheLocation}\\{spath}";
            if (File.Exists(filePath))
            {
                { return filePath; }
            }
            else
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(30);

                        using (var response = await client.GetAsync(Server + path, HttpCompletionOption.ResponseHeadersRead))
                        {
                            if (response.StatusCode != HttpStatusCode.OK) { return errorImg; }
                            response.EnsureSuccessStatusCode();

                            using (var stream = await response.Content.ReadAsStreamAsync())
                            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                            {
                                await stream.CopyToAsync(fileStream);
                            }
                        }
                        if (File.Exists(filePath)) { return filePath; }
                        else { return errorImg; }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return errorImg;
                }
            }
        }

        public async Task<string> DownloadDocument(string path, IProgress<double> progress = null)
        {
            if (String.IsNullOrEmpty(path)) { return errorImg; }

            string spath = path.Replace('/', '\\');
            string filePath = $"{defaulCacheLocation}\\{spath}";
            if (File.Exists(filePath))
            {
                { return filePath; }
            }
            else
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromMinutes(10);

                        using (var response = await client.GetAsync(Server + path, HttpCompletionOption.ResponseHeadersRead))
                        {
                            if (response.StatusCode != HttpStatusCode.OK) { return errorImg; }
                            response.EnsureSuccessStatusCode();

                            var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                            var canReportProgress = totalBytes != -1 && progress != null;

                            using (var stream = await response.Content.ReadAsStreamAsync())
                            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                            {
                                var buffer = new byte[8192];
                                var totalBytesRead = 0L;
                                int bytesRead;

                                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                                {
                                    await fileStream.WriteAsync(buffer, 0, bytesRead);
                                    totalBytesRead += bytesRead;

                                    if (canReportProgress)
                                    {
                                        var percentage = (double)totalBytesRead / totalBytes * 100;
                                        progress.Report(percentage);
                                        Debug.WriteLine($"Прогресс: {percentage:F1}% - {totalBytesRead}/{totalBytes} байт");
                                    }
                                }
                            }
                        }
                        if (File.Exists(filePath)) { return filePath; }
                        else { return errorImg; }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return errorImg;
                }
            }
        }

        public bool CheckForFileExists(string path)
        {
            string spath = path.Replace('/', '\\');
            string filePath = $"{defaulCacheLocation}\\{spath}";
            return File.Exists(filePath);
        }

        public string GetLocalFilePath(string path)
        {
            string spath = path.Replace('/', '\\');
            string filePath = $"{defaulCacheLocation}\\{spath}";
            return filePath;
        }
    }
}
