
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System;
using System.Threading.Tasks;
using System.Net;

using EagleServer.Exceptions;


namespace Eagle {

	/**
	This is a light weight demo server, @author JLC
	 */
	public class Server {

		private static Server server = null;

        private static HttpListener listner = null;

        private static readonly object sync = new object();

		private static string _port;

        private static bool _isHttp = false;

		private static bool _stop = false;

		private static Dictionary<string, Func<HttpListenerRequest, HttpListenerResponse, string>> postMappings = new Dictionary<string, Func<HttpListenerRequest, HttpListenerResponse, string>>();

		private static Dictionary<string, Func<HttpListenerRequest, HttpListenerResponse, string>> getMappings = new Dictionary<string, Func<HttpListenerRequest, HttpListenerResponse, string>>();

        private static Dictionary<string, Func<HttpListenerRequest, HttpListenerResponse, string>> putMappings = new Dictionary<string, Func<HttpListenerRequest, HttpListenerResponse, string>>();

        private static Dictionary<string, Func<HttpListenerRequest, HttpListenerResponse, string>> deleteMappings = new Dictionary<string, Func<HttpListenerRequest, HttpListenerResponse, string>>();

        private Server(){

			 if (!HttpListener.IsSupported)
            {
                throw new HttpListenerNotSupported();
            }

			listner = new HttpListener();

            if (!_isHttp)
            {
                if (_port == null || _port.Length == 0)
                {
                    listner.Prefixes.Add("https://+:8080/");
                }
                else
                {
                    listner.Prefixes.Add("https://+:" + _port + "/");
                }
            }
            else
            {
                if (_port == null || _port.Length == 0)
                {
                    listner.Prefixes.Add("http://+:8080/");
                }
                else
                {
                    listner.Prefixes.Add("http://+:" + _port + "/");
                }
            }


             listner.Start();

			Task.Run( () => {
				
				int count = 0;
				while (!_stop)
				{
					HttpListenerContext task = listner.GetContext();
					
					Task respondTask = Task.Run(() => {
                        HttpListenerContext ctx = task;
                        try
                        {
                            string path = ctx.Request.RawUrl;
                            string body = null;
                            if ("POST".Equals(ctx.Request.HttpMethod) && postMappings.ContainsKey(path))
                            {
                                body = postMappings[path](ctx.Request, ctx.Response);
                            }
                            else if ("GET".Equals(ctx.Request.HttpMethod) && getMappings.ContainsKey(path))
                            {
                                body = getMappings[path](ctx.Request, ctx.Response);
                            }
                            else if ("DELETE".Equals(ctx.Request.HttpMethod) && getMappings.ContainsKey(path))
                            {
                                body = deleteMappings[path](ctx.Request, ctx.Response);
                            }
                            else if ("PUT".Equals(ctx.Request.HttpMethod) && getMappings.ContainsKey(path))
                            {
                                body = putMappings[path](ctx.Request, ctx.Response);
                            }
                            reply(ctx.Response, body);
                        }
                        catch (HttpStatusAwareException ex)
                        {
                            reply(ctx.Response, ex);
                        } catch {
                            reply(ctx.Response, new HttpStatusAwareException(500, "internal server error"));
						} finally {
							count++;
						}
					} );
				}

				listner.Stop();

				listner.Close();
			});

		}

        public static bool isRunning()
        {
            return !_stop || listner.IsListening;
        }

        public static void stop()
        {
            _stop = true;
        }

        public static void useHttp(bool use)
        {
            _isHttp = use;
        }

        private static void reply(HttpListenerResponse response, string body){
			
			byte[] outBuffer = null;
			if(body != null){
				outBuffer = Encoding.ASCII.GetBytes(body);
				response.ContentLength64 = outBuffer.Length;
				response.OutputStream.Write(outBuffer, 0, outBuffer.Length);
			}

            response.StatusCode = 200;           
            response.OutputStream.Flush();
            response.OutputStream.Close();
            response.OutputStream.Dispose();
			response.Close();
		}

        private static void reply(HttpListenerResponse response, HttpStatusAwareException ex)
        {
            string body = ex.Body;

            byte[] outBuffer = null;
            if (body != null)
            {
                outBuffer = Encoding.ASCII.GetBytes(body);
                response.ContentLength64 = outBuffer.Length;
                response.OutputStream.Write(outBuffer, 0, outBuffer.Length);
            }

            response.StatusCode = ex.StatusCode;
            response.OutputStream.Flush();
            response.OutputStream.Close();
            response.OutputStream.Dispose();
            response.Close();
        }

        public static void port(string port){
			Server._port = port;
		}

		public static Server getInstance(){
			if(server == null ){
				lock(sync){
					if(server == null){
						server = new Server();
					}
				}
			}
			return Server.server;
		}

        public static Server startServerInstance()
        {
            return getInstance(); 
        }

        public static void post(string path, Func<HttpListenerRequest, HttpListenerResponse, string> func){

            if (server == null)
                throw new ServerNotStartedException();

            if (path != null && path.Length != 0)
				postMappings[path] = func;
			
		}

		public static void get(string path, Func<HttpListenerRequest, HttpListenerResponse, string> func){

			if(server == null)
                throw new ServerNotStartedException();

            if (path != null && path.Length != 0)
				getMappings[path] = func;
			
		}

        public static void put(string path, Func<HttpListenerRequest, HttpListenerResponse, string> func)
        {

            if (server == null)
                throw new ServerNotStartedException();

            if (path != null && path.Length != 0)
                putMappings[path] = func;

        }

        public static void delete(string path, Func<HttpListenerRequest, HttpListenerResponse, string> func)
        {

            if (server == null)
                throw new ServerNotStartedException();

            if (path != null && path.Length != 0)
                deleteMappings[path] = func;

        }


    }
}