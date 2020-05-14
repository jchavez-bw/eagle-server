
using EagleServer.Exceptions;
using NUnit.Framework;
using System.Collections.Generic;
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
        }


        [Test]
        public async Task TestPathParams()
        {

            post("/account/{accountId}", (request, response) => {

                var pathParameters = request.PathInfo.PathParameters;
                string accountId = pathParameters.accountId;

                return accountId;
            });

            HttpClient client = new HttpClient();
            var content = new StringContent("{\"test\": 0 }");

            var res = await client.PostAsync("http://localhost:8080/account/123456", content);
            Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
            Assert.AreEqual("123456", res.Content.ReadAsStringAsync().Result);

            delete("/account/{accountId}", (request, response) => {

                var pathParameters = request.PathInfo.PathParameters;
                string accountId = pathParameters.accountId;

                return accountId;
            });

            res = await client.DeleteAsync("http://localhost:8080/account/123456");
            Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
            Assert.AreEqual("123456", res.Content.ReadAsStringAsync().Result);

            get("/account/{accountId}", (request, response) => {

                var pathParameters = request.PathInfo.PathParameters;
                string accountId = pathParameters.accountId;

                return accountId;
            });

            res = await client.GetAsync("http://localhost:8080/account/123456");
            Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
            Assert.AreEqual("123456", res.Content.ReadAsStringAsync().Result);

            put("/account/{accountId}", (request, response) => {

                var pathParameters = request.PathInfo.PathParameters;
                string accountId = pathParameters.accountId;

                return accountId;
            });

            res = await client.PutAsync("http://localhost:8080/account/123456", content);
            Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
            Assert.AreEqual("123456", res.Content.ReadAsStringAsync().Result);

        }

        [Test]
        public async Task TestList()
        {

            post("/account/{accountId}/list", (request, response) => {
                return new List<string> { "this" };
            });

            HttpClient client = new HttpClient();
            var content = new StringContent("{\"test\": 0 }");

            var res = await client.PostAsync("http://localhost:8080/account/123456/list", content);
            Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
            Assert.AreEqual("[\"this\"]", res.Content.ReadAsStringAsync().Result);
        }

        [Test]
        public async Task TestQueryParams()
        {
            get("/account/{accountId}", (request, response) => {

                var pathParameters = request.PathInfo.PathParameters;
                var body = request.Body;

                string accountId = pathParameters.accountId;

                string taco = request.QueryParams.Get("taco");
                return $"{accountId} {taco}";
               
            });

            HttpClient client = new HttpClient();
            var content = new StringContent("{\"test\": 0 }");

            var res = await client.GetAsync("http://localhost:8080/account/123456?test=fish&taco=tuesday");
            Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
            Assert.AreEqual("123456 tuesday", res.Content.ReadAsStringAsync().Result);
        }

        [Test]
        public async Task TestGetSimple()
        {

            get("/test", (EagleRequest request, HttpListenerResponse response) => {
                return "success";
            });

            HttpClient client = new HttpClient();
            var content = new StringContent("{\"test\": 0 }");

            var res = await client.GetAsync("http://localhost:8080/test");
            Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
            Assert.AreEqual("success", res.Content.ReadAsStringAsync().Result);
        }

        [Test]
        public async Task TestNotFoundNoPath()
        {
            HttpClient client = new HttpClient();
            var content = new StringContent("{\"test\": 0 }");

            var response = await client.PostAsync("http://localhost:8080/NotFound", content);
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            Assert.AreEqual("Not Found", response.Content.ReadAsStringAsync().Result);
        }

        [Test]
        public async Task TestBody()
        {

            post("/body", (request, response) => {
                var body = request.Body;
                return body.test;
            });

            HttpClient client = new HttpClient();
            var content = new StringContent("{\"test\": 0 }");
            content.Headers.ContentType.MediaType = "application/json";

            var res = await client.PostAsync("http://localhost:8080/body", content);
            Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
            Assert.AreEqual("0", res.Content.ReadAsStringAsync().Result);
        }

        [Test]
        public async Task TestRoot()
        {

            post("/", (HttpListenerRequest request, HttpListenerResponse response) => {
                return "";
            });

            HttpClient client = new HttpClient();
            var content = new StringContent("{\"test\": 0 }");

            var res = await client.PostAsync("http://localhost:8080/", content);
            Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
        }

        [Test]
        public async Task TestNoReturn()
        {

            get("/NoReturn", (EagleRequest request, HttpListenerResponse response) => {});

            HttpClient client = new HttpClient();

            var res = await client.GetAsync("http://localhost:8080/NoReturn");
            Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
        }

        [Test]
        public async Task TestError()
        {

            post("/unauthorized", (EagleRequest body, HttpListenerResponse response) => {

                throw new HttpStatusAwareException(401, "access denied");

                return "Should not be here";
            });

            HttpClient client = new HttpClient();
            var content = new StringContent("{\"test\": 0 }");

            var res = await client.PostAsync("http://localhost:8080/unauthorized", content);
            Assert.AreEqual(HttpStatusCode.Unauthorized, res.StatusCode);
            Assert.AreEqual("access denied", res.Content.ReadAsStringAsync().Result);
        }

        [Test]
        public async Task StopServerTest()
        {
            post("/stop", (EagleRequest request, HttpListenerResponse response) => {

                Task.Run(() => {
                    Thread.Sleep(2000);
                    stop();
                });


                return "Shuting down server";
            });

            //WaitOnServerToStop();

            Assert.Pass();

        }

        
    }
}