using System;
using System.Linq;
using Microsoft.ServiceBus.Messaging;
using DocumentManagementCommon;
using Microsoft.Azure;

namespace documentManagementAdminWeb.Service
{
    public class ServiceBusService
    {
        private static readonly string connectionString = CloudConfigurationManager.GetSetting(AzureRelatedNames.ServiceBusConnectionString);
        private static readonly string queueName = AzureRelatedNames.ServiceBusQueueName;

        //private static void Test(int count = 0)
        //{
        //    DocumentBlobInfo blobInfo = new DocumentBlobInfo();

        //    blobInfo.BlobUri = new Uri("https://demodocument.blob.core.chinacloudapi.cn/demo-temp/d7d45bb3-c3cd-49d2-a8b0-b51273d22bc7.jpg");
        //    blobInfo.DocumentId = "1";

        //    SendMessage(JsonConvert.SerializeObject(blobInfo));


        //    Console.ReadLine();

        //    ReceiveMessage();
        //}

        public static void SendMessage(string m)
        {
            var client = QueueClient.CreateFromConnectionString(connectionString, queueName);
            var message = new BrokeredMessage(m);
            client.Send(message);            
        }

        public static void ReceiveMessage()
        {
            var client = QueueClient.CreateFromConnectionString(connectionString, queueName);

            //client.OnMessage(message =>
            //{
            //    Console.WriteLine(String.Format("Message body: {0}", message.GetBody<String>()));
            //    Console.WriteLine(String.Format("Message id: {0}", message.MessageId));
            //});

            var messages = client.ReceiveBatch(3);

            Console.WriteLine(String.Format("Message count: {0}", messages.Count()));

            foreach ( var item in messages)
            {
                Console.WriteLine(String.Format("Message body: {0}", item.GetBody<String>()));
                Console.WriteLine(String.Format("Message id: {0}", item.MessageId));

                item.Complete();
            }
            
        }
    }
}
