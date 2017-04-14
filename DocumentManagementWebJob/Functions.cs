using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure;
using DocumentManagementCommon;

namespace DocumentManagementWebJob
{
    public class Functions
    {
        private const string queueName = AzureRelatedNames.ServiceBusQueueName;

        public static void SBQueue2SBQueue(
      [ServiceBusTrigger(queueName)] string start,
      [ServiceBus(queueName + "1")] out string message,
      TextWriter log)
        {
            message = start + "-SBQueue2SBQueue";
            log.WriteLine("SBQueue2SBQueue: " + message);
        }
    }
}
