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
        private const string sbQueueName = AzureRelatedNames.ServiceBusQueueName;
        private const string sQueueName = AzureRelatedNames.StorageErrorQueueName;

        public static void SBQueue2SQueue(
      [ServiceBusTrigger(sbQueueName)] string error,
      [Queue(sQueueName)] out string message,
      TextWriter log)
        {
            message = string.Empty;

            var flag = SendEmail(error);
            if (!flag)
            {
                message = error;
            }

            log.WriteLine("SBQueue2SQueue: " + message);
        }

        private static bool SendEmail(string error)
        {
            //Todo: send email
            return false;
        }
    }
}
