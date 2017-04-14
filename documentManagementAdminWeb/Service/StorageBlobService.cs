using DocumentManagementCommon;
using DocumentManagementCommon.Services;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace documentManagementAdminWeb.Service
{
    public class StorageBlobService
    {
        private CloudBlobContainer tempBlobContainer;
        private CloudBlobContainer imagesBlobContainer;
        private CloudBlobContainer pdfsBlobContainer;
        private CloudBlobContainer excelsBlobContainer;

        public StorageBlobService()
        {
            InitializeStorages();
        }

        private void InitializeStorages()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting(AzureRelatedNames.StorageAccountConnectionName));

            var blobClient = storageAccount.CreateCloudBlobClient();
            tempBlobContainer = blobClient.GetContainerReference(CloudConfigurationManager.GetSetting(AzureRelatedNames.TempBlobContainerName));
            imagesBlobContainer = blobClient.GetContainerReference(CloudConfigurationManager.GetSetting(AzureRelatedNames.ImageBlobContainerName));
            excelsBlobContainer = blobClient.GetContainerReference(CloudConfigurationManager.GetSetting(AzureRelatedNames.ExcelBlobContainerName));
            pdfsBlobContainer = blobClient.GetContainerReference(CloudConfigurationManager.GetSetting(AzureRelatedNames.PdfBlobContainerName));
            
        }

        public void MoveBlob(DocumentBlobInfo blobInfo)
        {
            var sourceBlob = tempBlobContainer.GetAppendBlobReference(blobInfo.BlobName);
            CloudBlobContainer destinationContainer = GetDestinationContainerByExtension(blobInfo.FileExtension);
            if (destinationContainer != null)
            {
                var destinationBlob = destinationContainer.GetAppendBlobReference(blobInfo.BlobName);
                destinationBlob.StartCopy(sourceBlob);
                sourceBlob.Delete(DeleteSnapshotsOption.IncludeSnapshots);
            }
        }

        private CloudBlobContainer GetDestinationContainerByExtension(string extention)
        {
            CloudBlobContainer container;

            switch(extention.ToLower())
            {
                case ".jpg":
                    container = imagesBlobContainer;
                    break;
                case ".xlsx":
                    container = imagesBlobContainer;
                    break;
                case ".pdf":
                    container = imagesBlobContainer;
                    break;
                default:
                    container = null;
                    break;
            }

            return container;
        }

        public void DeleteBlobDocumentId(string documentId)
        {
            var document = DocumentDBService.Instance.GetDocumentById(documentId);
            if (document != null)
            {
                if (document.Status == DocumentStatus.Pending && !string.IsNullOrWhiteSpace(document.TempDocumentUrl))
                {
                    var blockBlob = tempBlobContainer.ServiceClient.GetBlobReferenceFromServer(new Uri(document.TempDocumentUrl));
                    blockBlob.Delete();                    
                }
            }
        }
    }
}