using DocumentManagementCommon;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace documentManagementAdminWeb.Service
{
    public class StorageQueueService
    {
        private CloudQueue documentQueue;

        public StorageQueueService()
        {
            InitializeStorages();
        }

        private void InitializeStorages()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting(AzureRelatedNames.StorageAccountConnectionName));

            var blobClient = storageAccount.CreateCloudBlobClient();

            var queueClient = storageAccount.CreateCloudQueueClient();
            documentQueue = queueClient.GetQueueReference(CloudConfigurationManager.GetSetting(AzureRelatedNames.StorageQueueContainerName));
        }

        public async Task SendQueueMessage(DocumentBlobInfo documentBlobInfo)
        {
            var queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(documentBlobInfo));
            await documentQueue.AddMessageAsync(queueMessage).ConfigureAwait(false);
        }
    }
}