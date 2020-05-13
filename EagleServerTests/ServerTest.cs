
using EagleServer.Exceptions;
using NUnit.Framework;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
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

            int count = 0;
            post("/", (HttpListenerRequest request, HttpListenerResponse response) => {

                count++;
                return "" + count;

            });

            post("/account/{accountId}", (request, response) => {

                var pathParameters = request.PathInfo.PathParameters;
                var body = request.Body;

                string test = body.test;

                string accountId = pathParameters.accountId;

                WriteLine($"accountId = {accountId} and test = {test}");

                return "";
            });

            get("/account/{accountId}", (request, response) => {

                var pathParameters = request.PathInfo.PathParameters;
                var body = request.Body;

                string accountId = pathParameters.accountId;

                WriteLine($"accountId = {accountId}");

                return "";
            });

            post("/dynamic", (request, response) => {

                var test = request.PathInfo.variableUrl;

                WriteLine(test);
                
                return "";
            });

            post("/stop", (EagleRequest request, HttpListenerResponse response) => {

                Task.Run(() => {
                    Thread.Sleep(2000);
                    stop();
                });
                

                return "Shuting down server";
            });

            post("/error", (EagleRequest body, HttpListenerResponse response) => {

                throw new HttpStatusAwareException(401, "access denied");


                return "Should not be here";
            });
        }


        [Test]
        public async Task Test1()
        {
            HttpClient client = new HttpClient();
            var content = new StringContent("{\"test\": 0 }");

            var response = await client.PostAsync("http://localhost:8080/account/123456", content);

            response = await client.GetAsync("http://localhost:8080/account/123456");

            response = await client.PostAsync("http://localhost:8080/dynamic", content);

            response = await client.PostAsync("http://localhost:8080/", content);

            response = await client.PostAsync("http://localhost:8080/NotFound", content);

            response = await client.PostAsync("http://localhost:8080/", content);

            


            Assert.Pass();
            
        }

        [Test]
        public async Task StopServerTest()
        {

            WaitOnServerToStop();

            Assert.Pass();

        }

        
    }
}