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
                FileSystemProvider = new DefaultFileProvider(Path.Combine(HostingEnvironment.WebRootPath, "SampleDocs")),
                AllowCopy = true,
                AllowCreate = true,
                AllowMove = true,
                AllowRemove = true,
                AllowRename = true,
                AllowUpload = true,
                AllowDownload = true
            };
            var processor = new FileSystemCommandProcessor(config);
            var result = processor.Execute(command, arguments);
            return result.GetClientCommandResult();
        }
    }
}