using documentManagementAdminWeb.Service;
using DocumentManagementCommon;
using DocumentManagementCommon.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace documentManagementAdminWeb.Controllers
{
    public class HomeController : Controller
    {
        private DocumentDBService documentService = DocumentDBService.Instance;
        private StorageBlobService blobService = new StorageBlobService();

        public ActionResult Index()
        {
            var result = documentService.GetUnApprovedDocuments();
            
            return View(result);
        }

        public ActionResult Approve(string id)
        {
            var doc = documentService.GetDocumentById(id);
            if (doc != null)
            {
                DocumentBlobInfo blobInfo = new DocumentBlobInfo
                {
                    BlobUri = new Uri(doc.TempDocumentUrl),
                    DocumentId = id
                };
                blobService.MoveBlob(blobInfo);
                //documentService.DeleteDocument(id).ConfigureAwait(true);
                documentService.UpdateDocument(id); //Set document as approved
            }


            return RedirectToAction("Index");
        }

        public ActionResult Reject(string id)
        {
            var doc = documentService.GetDocumentById(id);
            if (doc != null)
            {
                //Update document status
            }
            return RedirectToAction("Index");
        }

        //Get all blob info from document DB

        //Find item in document DB by documentId, 
        //Move blob from Temp container to new container by blob file type
        //Remove blob file from temp container
        //If move success, set status to Approved(1) in document DB
        //If move failed, send message to service bus

    }
}