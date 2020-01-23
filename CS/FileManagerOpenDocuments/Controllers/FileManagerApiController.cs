using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DevExtreme.AspNet.Mvc.FileManagement;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace T845880.Controllers {
    public class FileManagerApiController : ControllerBase {
        public IWebHostEnvironment HostingEnvironment { get; }
        public FileManagerApiController(IWebHostEnvironment hostingEnvironment) {
            HostingEnvironment = hostingEnvironment;
        }

        public object FileSystem(FileSystemCommand command, string arguments) {
            var config = new FileSystemConfiguration {
                Request = Request,
                FileSystemProvider = new DefaultFileProvider(
                    Path.Combine(HostingEnvironment.WebRootPath, "SampleDocs"),
                    (fileSystemItem, clientItem) => {
                        if (!clientItem.IsDirectory)
                            clientItem.CustomFields["url"] = GetFileItemUrl(fileSystemItem);
                    }
                ),
                AllowCopy = true,
                AllowCreate = true,
                AllowMove = true,
                AllowRemove = true,
                AllowRename = true,
                AllowUpload = true,
                AllowDownload = true,
                AllowedFileExtensions= new string[] {".xlsx", ".rtf", ".txt", ".docx", ".json", ".jpg" }
            };
          
            var processor = new FileSystemCommandProcessor(config);
            var result = processor.Execute(command, arguments);
            return result.GetClientCommandResult();
        }
        string GetFileItemUrl(FileSystemInfo fileSystemItem) {
            var relativeUrl = fileSystemItem.FullName
                .Replace(HostingEnvironment.WebRootPath, "")
                .Replace(Path.DirectorySeparatorChar, '/');
            return $"{Request.Scheme}://{Request.Host}{Request.PathBase}{relativeUrl}";
        }
    }
}