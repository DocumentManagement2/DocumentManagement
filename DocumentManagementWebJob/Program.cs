using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.ServiceBus;
using Microsoft.WindowsAzure;
using DocumentManagementCommon;
using Microsoft.Azure.WebJobs.ServiceBus;

namespace DocumentManagementWebJob
{
    class Program
    {
        private static string _servicesBusConnectionString;
        private static NamespaceManager _namespaceManager;
        
        static void Main()
        {
            _servicesBusConnectionString = CloudConfigurationManager.GetSetting(AzureRelatedNames.ServiceBusConnectionString);

            JobHostConfiguration config = new JobHostConfiguration();
            ServiceBusConfiguration serviceBusConfig = new ServiceBusConfiguration
            {
                ConnectionString = _servicesBusConnectionString
            };
            config.UseServiceBus(serviceBusConfig);

            JobHost host = new JobHost(config);
            host.RunAndBlock();
        }
    }
}
