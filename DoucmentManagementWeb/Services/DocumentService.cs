using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using DocumentManagementCommon;
using Microsoft.Azure;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace DoucmentManagementWeb.Services
{
    public class DocumentService
    {
        private const string DatabaseName = "demo-documentdb";
        private const string CollectionName = "demo_documentcollection";
        private const string TempBlobContainerName = "demo-temp";
        private const string ImageBlobContainerName = "demo-images";
        private const string ExcelBlobContainerName = "demo-excels";
        private const string PdfBlobContainerName = "demo-pdfs";
        private const string StorageQueueContainerName = "demo-queue";
        private const string StorageAccountConnectionName = "Microsoft.WindowsAzure.AzureStorage.ConnectionString";

        private CloudBlobContainer tempBlobContainer;
        private CloudBlobContainer imagesBlobContainer;
        private CloudBlobContainer pdfsBlobContainer;
        private CloudBlobContainer excelsBlobContainer;
        private CloudQueue documentQueue;
        private DocumentClient documentClient;

        private static readonly DocumentService instance = new DocumentService();

        private DocumentService()
        {
            InitializeDocumentDB();
            InitializeStorages();
        }

        private void InitializeDocumentDB()
        {
            var documentDBUri = CloudConfigurationManager.GetSetting("Microsoft.WindowsAzure.DocumentDB.Uri");
            var documentDBPrivateKey = CloudConfigurationManager.GetSetting("Microsoft.WindowsAzure.DocumentDB.PrivateKey");
            documentClient = new DocumentClient(new Uri(documentDBUri), documentDBPrivateKey);
        }

        private void InitializeStorages()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting(StorageAccountConnectionName));

            var blobClient = storageAccount.CreateCloudBlobClient();
            tempBlobContainer = blobClient.GetContainerReference(TempBlobContainerName);
            imagesBlobContainer = blobClient.GetContainerReference(ImageBlobContainerName);
            excelsBlobContainer = blobClient.GetContainerReference(ExcelBlobContainerName);
            pdfsBlobContainer = blobClient.GetContainerReference(PdfBlobContainerName);

            var queueClient = storageAccount.CreateCloudQueueClient();
            documentQueue = queueClient.GetQueueReference(StorageQueueContainerName);
        }

        public static DocumentService Instance
        {
            get
            {
                return instance;
            }
        }

        public IEnumerable<DocumentInfo> GetDocuments()
        {
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };

            return documentClient.CreateDocumentQuery<DocumentInfo>(
                    UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName), queryOptions);

        }

        public string SaveDocumentFile(HttpPostedFileBase importFile)
        {
            if (importFile != null && importFile.ContentLength != 0)
            {
                string blobName = Guid.NewGuid().ToString() + Path.GetExtension(importFile.FileName);
                var fileBlob = tempBlobContainer.GetBlockBlobReference(blobName);
                using (var fileStream = importFile.InputStream)
                {
                    fileBlob.UploadFromStream(fileStream);
                }

                return fileBlob.Uri.ToString();
            }

            return string.Empty;
        }

        public async Task CreateDocumentIfNotExists(DocumentInfo document)
        {
            try
            {
                await documentClient.ReadDocumentAsync(UriFactory.CreateDocumentUri(DatabaseName, CollectionName, document.Id));
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await documentClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName), document);
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task SendQueueMessage(DocumentBlobInfo documentBlobInfo)
        {
            var queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(documentBlobInfo));
            await documentQueue.AddMessageAsync(queueMessage).ConfigureAwait(false);
        }
    }
}