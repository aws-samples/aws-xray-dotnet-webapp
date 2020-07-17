using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Util;
using Microsoft.AspNetCore.Mvc;
using SampleASPNETCoreApplication.Models;

namespace SampleASPNETCoreApplication.Controllers
{
    [Produces("application/json")]
    [Route("api/Products")]
    public class ProductsController : Controller
    {
        static ProductsController()
        {
           
            LazyDdbClient = new Lazy<AmazonDynamoDBClient>(() =>
            {
                var client = new AmazonDynamoDBClient(RegionEndpoint.USEast1); // When running locally, configure with desired region and comment above line of client creation.

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
                var product = QueryProduct(id);

                // Trace out-going HTTP request
                MakeHttpWebRequest(id);

                // Trace SQL query
                // QuerySql(id);

                return product.ToString();// Ok(product);
            }
            catch (ProductNotFoundException e)
            {
                return "Product not found !";// NotFound();
            }
        }

        // POST: api/Products
        [HttpPost]
        public void Post(Product product)//[FromBody]string value)
        {
           AddProduct<Document>(product).Wait();
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        private static void MakeHttpWebRequest(int id)
        {
            HttpWebRequest request = null;
            request = (HttpWebRequest) WebRequest.Create("http://www.amazon.com");
            using (var response = request.GetResponse()) { }
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
            using (var sqlCommand = new SqlCommand("SELECT " + id, sqlConnection))
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
