using FaRequestLoggingApi.Models;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace FaRequestLoggingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FASLoggingController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> PostAsync([FromBody] object body)
        {
            //await SaveS3(body);
            await SaveDynamoDb(body);
            return Ok("Logged");
        }

        private async Task SaveS3(object body)
        {
            var bucketName = "fa-siza-textract-src";
            var objectName = Guid.NewGuid().ToString() + ".json";
            using (var client = new AmazonS3Client(Amazon.RegionEndpoint.AFSouth1))
            {
                var request = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = objectName,
                    ContentBody = body.ToString()
                };
                var response = await client.PutObjectAsync(request);
            }
        }

        private async Task SaveDynamoDb(object body)
        {
            AmazonDynamoDBConfig clientConfig = new AmazonDynamoDBConfig();
            clientConfig.RegionEndpoint = RegionEndpoint.AFSouth1;
            AmazonDynamoDBClient client = new AmazonDynamoDBClient(clientConfig);
            if (body != null)
            {
                var fasRequestLog = JsonConvert.DeserializeObject<RequestLog>(body.ToString() ?? "");
                if (fasRequestLog != null)
                {
                    Table table = Table.LoadTable(client, "fas-logs");

                    var log = new Document();
                    log["TransactionId"] = fasRequestLog.TransactionId.ToString();
                    log["LogTime"] = fasRequestLog.LogTime.ToString();
                    log["TemplateId"] = fasRequestLog.TemplateId.ToString();
                    log["ServiceName"] = fasRequestLog.ServiceName;
                    log["MachineName"] = fasRequestLog.MachineName;
                    log["LogType"] = fasRequestLog.LogType;
                    log["ServiceRequestStatus"] = fasRequestLog.ServiceRequestStatus;
                    log["RequestContent"] = fasRequestLog.RequestContent;
                    log["ResponseContent"] = fasRequestLog.ResponseContent;
                    log["ExceptionContent"] = fasRequestLog.ExceptionContent;

                    await table.PutItemAsync(log);
                }
            }
        }
    }
}
