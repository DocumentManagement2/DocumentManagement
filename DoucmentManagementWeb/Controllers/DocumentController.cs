using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DocumentManagementCommon;
using DoucmentManagementWeb.Services;

namespace DoucmentManagementWeb.Controllers
{
    public class DocumentController : Controller
    {
        private readonly DocumentService documentService;

        public DocumentController()
        {
            documentService = DocumentService.Instance;
        }

        public async Task<ActionResult> DocumentList(string fileType)
        {
            return View(new List<DocumentInfo>(documentService.GetDocuments()));
        }

        public async Task<ActionResult> AddDocument()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AddDocument(DocumentInfo document, HttpPostedFileBase importFile)
        {
            var tempBlobUri = documentService.SaveDocumentFile(importFile);
            if (!string.IsNullOrWhiteSpace(tempBlobUri))
            {
                document.TempDocumentUrl = tempBlobUri;
                await documentService.CreateDocumentIfNotExists(document).ConfigureAwait(false);
                await documentService.SendQueueMessage(new DocumentBlobInfo() { BlobUri = new Uri(tempBlobUri), DocumentId = document.Id });
            }

            return RedirectToAction("DocumentList");
        }
    }
}