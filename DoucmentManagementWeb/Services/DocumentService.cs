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
            var documentDBUri = CloudConfigurationManager.GetSetting(AzureRelatedNames.DocumentDBUri);
            var documentDBPrivateKey = CloudConfigurationManager.GetSetting(AzureRelatedNames.DocumentDBPrivateKey);
            documentClient = new DocumentClient(new Uri(documentDBUri), documentDBPrivateKey);
        }

        private void InitializeStorages()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting(AzureRelatedNames.StorageAccountConnectionName));

            var blobClient = storageAccount.CreateCloudBlobClient();
            tempBlobContainer = blobClient.GetContainerReference(CloudConfigurationManager.GetSetting(AzureRelatedNames.TempBlobContainerName));
            imagesBlobContainer = blobClient.GetContainerReference(CloudConfigurationManager.GetSetting(AzureRelatedNames.ImageBlobContainerName));
            excelsBlobContainer = blobClient.GetContainerReference(CloudConfigurationManager.GetSetting(AzureRelatedNames.ExcelBlobContainerName));
            pdfsBlobContainer = blobClient.GetContainerReference(CloudConfigurationManager.GetSetting(AzureRelatedNames.PdfBlobContainerName));

            var queueClient = storageAccount.CreateCloudQueueClient();
            documentQueue = queueClient.GetQueueReference(CloudConfigurationManager.GetSetting(AzureRelatedNames.StorageQueueContainerName));
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
            var queryOptions = new FeedOptions { MaxItemCount = -1 };
            var sqlQuery = new SqlQuerySpec();
            sqlQuery.QueryText = "select * from documents d where d.Status=@status";
            sqlQuery.Parameters.Add(new SqlParameter("@status", (int)DocumentStatus.Approved));

            return documentClient.CreateDocumentQuery<DocumentInfo>(
                    UriFactory.CreateDocumentCollectionUri(CloudConfigurationManager.GetSetting(AzureRelatedNames.DatabaseName),
                    CloudConfigurationManager.GetSetting(AzureRelatedNames.CollectionName)), sqlQuery, queryOptions);

        }

        public DocumentInfo GetDocumentById(string documentId)
        {
            var queryOptions = new FeedOptions { MaxItemCount = -1 };
            var sqlQuery = new SqlQuerySpec();
            sqlQuery.QueryText = "select * from documents ds where ds.id=@id";
            sqlQuery.Parameters.Add(new SqlParameter("@id", documentId));

            var documents = documentClient.CreateDocumentQuery<DocumentInfo>(
                    UriFactory.CreateDocumentCollectionUri(CloudConfigurationManager.GetSetting(AzureRelatedNames.DatabaseName),
                    CloudConfigurationManager.GetSetting(AzureRelatedNames.CollectionName)), sqlQuery, queryOptions);

            return documents.AsEnumerable().FirstOrDefault();
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
                await documentClient.ReadDocumentAsync(UriFactory.CreateDocumentUri(CloudConfigurationManager.GetSetting(AzureRelatedNames.DatabaseName),
                    CloudConfigurationManager.GetSetting(AzureRelatedNames.CollectionName), document.Id));
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await documentClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(CloudConfigurationManager.GetSetting(AzureRelatedNames.DatabaseName),
                        CloudConfigurationManager.GetSetting(AzureRelatedNames.CollectionName)), document);
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

        public async Task DeleteDocument(string documentId)
        {
            var document = GetDocumentById(documentId);
            if (document != null)
            {
                if (document.Status == DocumentStatus.Pending && !string.IsNullOrWhiteSpace(document.TempDocumentUrl))
                {
                    var blockBlob = tempBlobContainer.ServiceClient.GetBlobReferenceFromServer(new Uri(document.TempDocumentUrl));
                    blockBlob.Delete();

                    await documentClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(CloudConfigurationManager.GetSetting(AzureRelatedNames.DatabaseName),
                        CloudConfigurationManager.GetSetting(AzureRelatedNames.CollectionName), documentId));
                }
            }
        }
    }
}