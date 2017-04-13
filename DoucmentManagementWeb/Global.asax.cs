using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.Azure;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DoucmentManagementWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private const string DatabaseName = "demo-documentdb";
        private const string CollectionName = "demo_documentcollection";
        private const string TempBlobContainerName = "demo-temp";
        private const string ImageBlobContainerName = "demo-images";
        private const string ExcelBlobContainerName = "demo-excels";
        private const string PdfBlobContainerName = "demo-pdfs";
        private const string StorageQueueContainerName = "demo-queue";
        private const string StorageAccountConnectionName = "Microsoft.WindowsAzure.AzureStorage.ConnectionString";

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            InitializeDocumentDB().Wait();
            InitializeStorage();
        }

        private async Task InitializeDocumentDB()
        {
            try
            {
                var documentDBUri = CloudConfigurationManager.GetSetting("Microsoft.WindowsAzure.DocumentDB.Uri");
                var documentDBPrivateKey = CloudConfigurationManager.GetSetting("Microsoft.WindowsAzure.DocumentDB.PrivateKey");
                var documentClient = new DocumentClient(new Uri(documentDBUri), documentDBPrivateKey);

                await CreateDataBaseIfNotExists(documentClient).ConfigureAwait(false);
                await CreateDocumentCollectionIfNotExists(documentClient).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void InitializeStorage()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting(StorageAccountConnectionName));
            var blobClient = storageAccount.CreateCloudBlobClient();
            CreateBlobContainerIfNotExists(blobClient, TempBlobContainerName);
            CreateBlobContainerIfNotExists(blobClient, ImageBlobContainerName);
            CreateBlobContainerIfNotExists(blobClient, ExcelBlobContainerName);
            CreateBlobContainerIfNotExists(blobClient, PdfBlobContainerName);

            var queueClient = storageAccount.CreateCloudQueueClient();
            var blobnameQueue = queueClient.GetQueueReference(StorageQueueContainerName);
            blobnameQueue.CreateIfNotExists();

        }

        private async Task CreateDataBaseIfNotExists(DocumentClient documentClient)
        {
            try
            {
                await documentClient.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(DatabaseName));
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await documentClient.CreateDatabaseAsync(new Database { Id = DatabaseName });
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task CreateDocumentCollectionIfNotExists(DocumentClient documentClient)
        {
            try
            {
                await documentClient.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName));
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    DocumentCollection collectionInfo = new DocumentCollection();
                    collectionInfo.Id = CollectionName;

                    collectionInfo.IndexingPolicy = new IndexingPolicy(new RangeIndex(DataType.String) { Precision = -1 });

                    await documentClient.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(DatabaseName),
                        collectionInfo,
                        new RequestOptions { OfferThroughput = 400 });
                }
                else
                {
                    throw;
                }
            }
        }

        private void CreateBlobContainerIfNotExists(CloudBlobClient blobClient, string name)
        {
            try
            {
                var blobContainer = blobClient.GetContainerReference(name);
                if (blobContainer.CreateIfNotExists())
                {
                    blobContainer.SetPermissions(
                        new BlobContainerPermissions
                        {
                            PublicAccess = BlobContainerPublicAccessType.Blob
                        });
                }
            }
           catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
