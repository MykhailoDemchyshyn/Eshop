using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace OrderProcessor
{
    public static class OrderProcessor
    {
        [FunctionName("OrderProcessor")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(databaseName: "EShopdb", collectionName: "container",
                ConnectionStringSetting = "CosmosDbConnectionString"
            )]IAsyncCollector<dynamic> documentsOut,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                await documentsOut.AddAsync(new
                {
                    Id = Guid.NewGuid(),
                    message = requestBody
                });

                return new OkObjectResult(requestBody);
        }
    }
}
