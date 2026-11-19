using DevExtreme.AspNet.Mvc.FileManagement;
using Microsoft.AspNetCore.Mvc;

namespace FileManagerOpenDocuments.Controllers {
    public class FileManagerApiController : ControllerBase {
        public IWebHostEnvironment HostingEnvironment { get; }
        public FileManagerApiController(IWebHostEnvironment hostingEnvironment) {
            HostingEnvironment = hostingEnvironment;
        }

        public object FileSystem(FileSystemCommand command, string arguments) {
            var config = new FileSystemConfiguration {
                Request = Request,
                FileSystemProvider = new PhysicalFileSystemProvider(Path.Combine(HostingEnvironment.WebRootPath, "SampleDocs")),
                AllowCopy = true,
                AllowCreate = true,
                AllowMove = true,
                AllowDelete = true,
                AllowRename = true,
                AllowUpload = true,
                AllowDownload = true,
                AllowedFileExtensions= new string[] {".xlsx", ".rtf", ".txt", ".docx", ".json", ".jpg" }
            };
          
            var processor = new FileSystemCommandProcessor(config);
            var result = processor.Execute(command, arguments);
            return result.GetClientCommandResult();
        }
    }
}