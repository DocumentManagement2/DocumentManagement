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

namespace DocumentManagementCommon.Services
{
    public class DocumentDBService
    {
        private DocumentClient documentClient;

        private static readonly DocumentDBService instance = new DocumentDBService();

        private DocumentDBService()
        {
            InitializeDocumentDB();
        }

        private void InitializeDocumentDB()
        {
            var documentDBUri = CloudConfigurationManager.GetSetting(AzureRelatedNames.DocumentDBUri);
            var documentDBPrivateKey = CloudConfigurationManager.GetSetting(AzureRelatedNames.DocumentDBPrivateKey);
            documentClient = new DocumentClient(new Uri(documentDBUri), documentDBPrivateKey);
        }


        public static DocumentDBService Instance
        {
            get
            {
                return instance;
            }
        }

        public IEnumerable<DocumentInfo> GetUnApprovedDocuments()
        {
            var queryOptions = new FeedOptions { MaxItemCount = -1 };
            var sqlQuery = new SqlQuerySpec();
            sqlQuery.QueryText = "select * from documents d where d.Status=@Status1 or d.Status=@Status2";
            sqlQuery.Parameters.Add(new SqlParameter("@Status1", (int)DocumentStatus.Pending )); 
            sqlQuery.Parameters.Add(new SqlParameter("@Status2", (int)DocumentStatus.Rejected));

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

        public void ApproveDocument(string documentId)
        {
            UpdateDocument(documentId, DocumentStatus.Approved).Wait();
        }
        public void RejectDocument(string documentId)
        {
            UpdateDocument(documentId, DocumentStatus.Rejected).Wait();
        }

        public async Task UpdateDocument(string documentId, DocumentStatus status)
        {
            var doc = GetDocumentById(documentId);
            doc.Status = status;
            if (doc!=null)
            {
                await documentClient.ReplaceDocumentAsync(doc._self, doc).ConfigureAwait(false);
            }
        }

        public async Task DeleteDocument(string documentId)
        {
            await documentClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(CloudConfigurationManager.GetSetting(AzureRelatedNames.DatabaseName),
                CloudConfigurationManager.GetSetting(AzureRelatedNames.CollectionName), documentId));
        }
    }
}