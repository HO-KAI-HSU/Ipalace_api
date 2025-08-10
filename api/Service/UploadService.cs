namespace npm.api.API.Service
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Web.Configs;

    public class UploadService
    {
        public string FileRoot { get => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Config.Instance.CDNRoot); }

        public async Task<(string FileName, string Url)> Upload(HttpRequestMessage request, params string[] directory)
        {
            var root = Path.Combine(new string[] { FileRoot }.Concat(directory).ToArray());
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }

            var provider = new MultipartMemoryStreamProvider();
            await request.Content.ReadAsMultipartAsync(provider);
            foreach (var content in provider.Contents)
            {
                var fileBytes = await content.ReadAsByteArrayAsync();
                var srcFileName = content.Headers.ContentDisposition.FileName.Trim('\"');
                var fileName = Guid.NewGuid() + Path.GetExtension(srcFileName).ToLower();
                using (var output = new FileStream(Path.Combine(root, fileName), FileMode.Create, FileAccess.Write))
                {
                    await output.WriteAsync(fileBytes, 0, fileBytes.Length);
                }

                return (fileName, Path.Combine(directory.Concat(new string[] { fileName }).ToArray()).Replace("\\", "/"));
            }

            return (null, null);
        }

        public string UploadThumbnail(string path, string content)
        {
            var ext = Path.GetExtension(path);
            var fileName = path.Replace(ext, ".jpg");
            File.WriteAllBytes(Path.Combine(FileRoot, fileName), Convert.FromBase64String(content));
            return fileName;
        }

        public string GetFullPath(params string[] directory)
        {
            return Path.Combine(new string[] { FileRoot }.Concat(directory).ToArray());
        }
    }
}