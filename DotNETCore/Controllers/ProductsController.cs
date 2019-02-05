using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Util;
using Amazon.XRay.Recorder.Core;
using Amazon.XRay.Recorder.Handlers.SqlServer;
using Amazon.XRay.Recorder.Handlers.System.Net;
using Microsoft.AspNetCore.Mvc;
using SampleEBASPNETCoreApplication.Models;

namespace SampleEBASPNETCoreApplication.Controllers
{
    [Produces("application/json")]
    [Route("api/Products")]
    public class ProductsController : Controller
    {
        static ProductsController()
        {
           
            LazyDdbClient = new Lazy<AmazonDynamoDBClient>(() =>
            {
                var client = new AmazonDynamoDBClient(EC2InstanceMetadata.Region ?? RegionEndpoint.USWest2);

                //var client = new AmazonDynamoDBClient(RegionEndpoint.USWest2); // When running locally, configure with desired region and comment above line of client creation.

                return client;
            });

            LazyTable = new Lazy<Table>(() =>
            {
                var tableName = "SampleProduct";
                return Table.LoadTable(LazyDdbClient.Value, tableName);
            });

        }
        private static readonly Lazy<AmazonDynamoDBClient> LazyDdbClient;
        private static readonly Lazy<Table> LazyTable;
        // GET: api/Products
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Products/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            try
            {
                AWSXRayRecorder.Instance.AddAnnotation("Get", id);

                var product = AWSXRayRecorder.Instance.TraceMethod<Product>("QueryProduct", () => QueryProduct(id));

                // Trace out-going HTTP request
                AWSXRayRecorder.Instance.TraceMethod("Outgoing Http Web Request", () => MakeHttpWebRequest(id));

                CustomSubsegment(); // create custom Subsegment

                // Trace SQL query
                //  AWSXRayRecorder.Instance.TraceMethod("Query SQL", () => QuerySql(id));

                return product.ToString();// Ok(product);
            }
            catch (ProductNotFoundException e)
            {
                return "Product not found !";// NotFound();
            }
        }

        private void CustomSubsegment()
        {
            try
            {
                AWSXRayRecorder.Instance.BeginSubsegment("CustomSubsegment");
                // business logic
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

        // POST: api/Products
        [HttpPost]
        public void Post(Product product)//[FromBody]string value)
        {
           AWSXRayRecorder.Instance.TraceMethodAsync("AddProduct", () => AddProduct<Document>(product));
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }
        private static void MakeHttpWebRequest(int id)
        {
            AWSXRayRecorder.Instance.AddAnnotation("WebRequestCall", id);
            HttpWebRequest request = null;
            request = (HttpWebRequest) WebRequest.Create("http://www.amazon.com");
            request.GetResponseTraced();
        }

        private Product QueryProduct(int id)
        {
            var item = LazyTable.Value.GetItemAsync(id).Result;
            if (item == null)
            {
                throw new ProductNotFoundException("Can't find a product with id = " + id);
            }

            return BuildProduct(item);
        }

        private async Task<Document> AddProduct<TResult>(Product product)
        {
            var document = new Document();
            document["Id"] = product.Id;
            document["Name"] = product.Name;
            document["Price"] = product.Price;

            return await LazyTable.Value.PutItemAsync(document);
        }

        private Product BuildProduct(Document document)
        {
            var product = new Product();
            product.Id = document["Id"].AsInt();
            product.Name = document["Name"].AsString();
            product.Price = document["Price"].AsDecimal();
            return product;
        }

        private void QuerySql(int id)
        {
            var connectionString = ""; // Configure Connection string -> Format : "Data Source=(RDS endpoint),(port number);User ID=(your user name);Password=(your password);"
            using (var sqlConnection = new SqlConnection(connectionString))
            using (var sqlCommand = new TraceableSqlCommand("SELECT " + id, sqlConnection))
            {
                sqlCommand.Connection.Open();
                sqlCommand.ExecuteNonQuery();
            }
        }
        private class ProductNotFoundException : Exception
        {
            public ProductNotFoundException(string message) : base(message) { }
        }
    }
}