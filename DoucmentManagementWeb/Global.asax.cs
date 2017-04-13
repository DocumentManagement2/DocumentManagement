using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using DocumentManagementCommon;
using Microsoft.Azure;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DoucmentManagementWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
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
                var documentDBUri = CloudConfigurationManager.GetSetting(AzureRelatedNames.DocumentDBUri);
                var documentDBPrivateKey = CloudConfigurationManager.GetSetting(AzureRelatedNames.DocumentDBPrivateKey);
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
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting(AzureRelatedNames.StorageAccountConnectionName));
            var blobClient = storageAccount.CreateCloudBlobClient();
            CreateBlobContainerIfNotExists(blobClient, CloudConfigurationManager.GetSetting(AzureRelatedNames.TempBlobContainerName));
            CreateBlobContainerIfNotExists(blobClient, CloudConfigurationManager.GetSetting(AzureRelatedNames.ImageBlobContainerName));
            CreateBlobContainerIfNotExists(blobClient, CloudConfigurationManager.GetSetting(AzureRelatedNames.ExcelBlobContainerName));
            CreateBlobContainerIfNotExists(blobClient, CloudConfigurationManager.GetSetting(AzureRelatedNames.PdfBlobContainerName));

            var queueClient = storageAccount.CreateCloudQueueClient();
            var blobnameQueue = queueClient.GetQueueReference(CloudConfigurationManager.GetSetting(AzureRelatedNames.StorageQueueContainerName));
            blobnameQueue.CreateIfNotExists();

        }

        private async Task CreateDataBaseIfNotExists(DocumentClient documentClient)
        {
            try
            {
                await documentClient.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(CloudConfigurationManager.GetSetting(AzureRelatedNames.DatabaseName)));
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await documentClient.CreateDatabaseAsync(new Database { Id = CloudConfigurationManager.GetSetting(AzureRelatedNames.DatabaseName) });
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
                await documentClient.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(CloudConfigurationManager.GetSetting(AzureRelatedNames.DatabaseName),
                    CloudConfigurationManager.GetSetting(AzureRelatedNames.CollectionName)));
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    DocumentCollection collectionInfo = new DocumentCollection();
                    collectionInfo.Id = CloudConfigurationManager.GetSetting(AzureRelatedNames.CollectionName);

                    collectionInfo.IndexingPolicy = new IndexingPolicy(new RangeIndex(DataType.String) { Precision = -1 });

                    await documentClient.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(CloudConfigurationManager.GetSetting(AzureRelatedNames.DatabaseName)),
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
