using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System.Web.Http;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Util;
using Amazon.XRay.Recorder.Core;
using Amazon.XRay.Recorder.Handlers.SqlServer;
using Amazon.XRay.Recorder.Handlers.System.Net;
using SampleEBWebApplication.Models;

namespace SampleEBWebApplication.Controllers
{
    public class ProductsController : ApiController
    {
        private static readonly Lazy<AmazonDynamoDBClient> LazyDdbClient = new Lazy<AmazonDynamoDBClient>(() =>
        {
            var client = new AmazonDynamoDBClient(EC2InstanceMetadata.Region ?? RegionEndpoint.USWest2);
            return client;
        });
        
        private static readonly Lazy<Table> LazyTable = new Lazy<Table>(() =>
        {
            var tableName = ConfigurationManager.AppSettings["DDB_TABLE_NAME"];
            return Table.LoadTable(LazyDdbClient.Value, tableName);
        });

        public IHttpActionResult GetProduct(int id)
        {
            try
            {
                // Trace DynamoDB requests
                var product = AWSXRayRecorder.Instance.TraceMethod<Product>("QueryProduct", () => QueryProduct(id));

                // Trace out-going HTTP request
                AWSXRayRecorder.Instance.TraceMethod("Outgoing Http Request", MakeHttpRequest);

                // Trace SQL query
                // AWSXRayRecorder.Instance.TraceMethod("Query SQL", () => QuerySql(id));

                CustomSegbsegment(); // generate custom subsegment

                return Ok(product);
            } 
            catch(ProductNotFoundException)
            {
                return NotFound();
            }
        }

        private void CustomSegbsegment()
        {
            try
            {
                AWSXRayRecorder.Instance.BeginSubsegment("CustomSubsegment");
                // Custom logic
            }
            catch (Exception e)
            {
                AWSXRayRecorder.Instance.AddException(e);
            }
            finally
            {
                AWSXRayRecorder.Instance.EndSubsegment();
            }
        }

        public IHttpActionResult PostProduct(Product product)
        {
            try
            {
                AWSXRayRecorder.Instance.TraceMethod("AddProduct", () => AddProduct(product));
                return StatusCode(HttpStatusCode.Created);
            }
            catch (Exception)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }

        private void MakeHttpRequest()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.amazon.com");
            request.GetResponseTraced();
        }

        private void QuerySql(int id)
        {
            var connectionString = ConfigurationManager.AppSettings["RDS_CONNECTION_STRING"];
            using (var sqlConnection = new SqlConnection(connectionString))
            using (var sqlCommand = new TraceableSqlCommand("SELECT " + id, sqlConnection))
            {
                sqlCommand.Connection.Open();
                sqlCommand.ExecuteNonQuery();
            } 
        }

        private Product QueryProduct(int id)
        {
            var item = LazyTable.Value.GetItem(id);
            if (item == null)
            {
                throw new ProductNotFoundException("Can't find a product with id = " + id);
            }

            return BuildProduct(item);
        }

        private void AddProduct(Product product)
        {
            var document = new Document();
            document["Id"] = product.Id;
            document["Name"] = product.Name;
            document["Price"] = product.Price;

            LazyTable.Value.PutItem(document);
        }

        private Product BuildProduct(Document document)
        {
            var product = new Product();
            product.Id = document["Id"].AsInt();
            product.Name = document["Name"].AsString();
            product.Price = document["Price"].AsDecimal();
            return product;
        }

        private class ProductNotFoundException : Exception
        {
            public ProductNotFoundException(string message) : base(message) { }
        }
    }
}
