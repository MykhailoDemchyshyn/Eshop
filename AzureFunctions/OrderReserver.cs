using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;


namespace Company.Function
{
    public static class OrderReserver
    {
        [FunctionName("OrderReserver")]
        public static void Run([ServiceBusTrigger("eshop", Connection = "eshopsb_SERVICEBUS")]string myQueueItem, ILogger log)
        {
            var name = Guid.NewGuid().ToString("n");
            CreateBlob(name + ".json", myQueueItem, log);
        }

        private static async Task CreateBlob(string name, string data, ILogger log)
        {
            string connectionString;
            CloudStorageAccount storageAccount;
            CloudBlobClient client;
            CloudBlobContainer container;
            CloudBlockBlob blob;

            connectionString = "DefaultEndpointsProtocol=https;AccountName=myeshopstorageaccount;AccountKey=vsy07yFNwJ6BJrO8Bfo0AGgLxzguxFNwNcuV075r7l78DBz2AC0gesKaid+xXO4885mYuxDFeJuIRm7Ra5ez/g==;EndpointSuffix=core.windows.net";
            storageAccount = CloudStorageAccount.Parse(connectionString);

            client = storageAccount.CreateCloudBlobClient();

            container = client.GetContainerReference("items");

            await container.CreateIfNotExistsAsync();

            blob = container.GetBlockBlobReference(name);
            blob.Properties.ContentType = "application/json";

            using (Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            {
                await blob.UploadFromStreamAsync(stream);
            }
        }
    }
}
