using DevExpress.AspNetCore.Spreadsheet;
using FileManagerOpenDocuments.Models;
using Microsoft.AspNetCore.Mvc;

namespace FileManagerOpenDocuments.Controllers {
    public class HomeController : Controller {
        private readonly string rootPath;

        public HomeController(IWebHostEnvironment hostingEnvironment) {
            rootPath = Path.Combine(hostingEnvironment.WebRootPath, "SampleDocs");
        }

        [HttpPost]
        public IActionResult OpenDocInSpreadsheet(string filePath) {
            if (string.IsNullOrWhiteSpace(filePath)) {
                return BadRequest("A document path is required.");
            }

            string path = Path.GetFullPath(Path.Combine(rootPath, filePath));
            string relativePath = Path.GetRelativePath(rootPath, path);
            if (Path.IsPathRooted(relativePath) || relativePath == ".." ||
                relativePath.StartsWith($"..{Path.DirectorySeparatorChar}") ||
                !string.Equals(Path.GetExtension(path), ".xlsx", StringComparison.OrdinalIgnoreCase)) {
                return BadRequest("Select an XLSX document in SampleDocs.");
            }
            if (!System.IO.File.Exists(path)) {
                return NotFound("The document was not found.");
            }

            return PartialView("SpreadsheetPartial", new DocumentModel {
                DocumentID = relativePath,
                FileBytes = System.IO.File.ReadAllBytes(path)
            });
        }

        [HttpPost]
        [HttpGet]
        public IActionResult DxDocRequest() {
            return SpreadsheetRequestProcessor.GetResponse(HttpContext);
        }
    }
}
