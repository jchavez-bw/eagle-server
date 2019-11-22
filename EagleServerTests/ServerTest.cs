using NUnit.Framework;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using static Eagle.Server;
using static System.Console;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {

            //port(8181);
            useHttp(true);
            startServerInstance();

            post("/", (HttpListenerRequest request, HttpListenerResponse response) => {

                return "";

            });

            post("/dynamic", (dynamic body, HttpListenerResponse response) => {

                int test = body.test;

                WriteLine(test);
                
                return "";
            });
        }

        [Test]
        public async Task Test1()
        {
            HttpClient client = new HttpClient();
            var content = new StringContent("{\"test\": 0 }");

            var response = await client.PostAsync("http://localhost:8080/dynamic", content);

            response = await client.PostAsync("http://localhost:8080/", content);

            response = await client.PostAsync("http://localhost:8080/NotFound", content);

            response = await client.PostAsync("http://localhost:8080/", content);


            Assert.Pass();
            
        }
    }
}