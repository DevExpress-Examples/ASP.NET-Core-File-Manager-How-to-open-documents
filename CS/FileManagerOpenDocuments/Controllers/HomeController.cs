using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.AspNetCore.Spreadsheet;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using T845880.Models;

namespace T845880.Controllers {
    public class HomeController : Controller {
        public IWebHostEnvironment HostingEnvironment { get; }
        public string RootPath { get; set; }
        public HomeController(IWebHostEnvironment hostingEnvironment) {
            HostingEnvironment = hostingEnvironment;
            RootPath = Path.Combine(HostingEnvironment.WebRootPath, "SampleDocs");
        }
        public IActionResult Index() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult OpenDocInSpreadsheet(string filePath) {
            return PartialView("SpreadsheetPartial", GetDocumentModel(filePath));
        }
        [HttpPost]
        [HttpGet]
        public IActionResult DxDocRequest() {
            return SpreadsheetRequestProcessor.GetResponse(HttpContext);
        }

        private DocumentModel GetDocumentModel(string filePath) {
            string path = Path.Combine(RootPath, filePath);
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            DocumentModel model = new DocumentModel() { DocumentID = Path.GetFileName(path), FileBytes = fileBytes };
            return model;
        }
    }

    public class DocumentModel {
        public string DocumentID { get; set; }
        public byte[] FileBytes { get; set; }
    }
}
