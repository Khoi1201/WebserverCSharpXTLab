using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebserverCSharpXTLab
{
    public class Server
    {
        private HttpListener listener;

        public async Task StartServer()
        {
            if (!HttpListener.IsSupported)
            {
                throw new Exception("HttpListener is not supported");
            }

            listener = new HttpListener();
            var prefixes = new string[] { "http://*:8080/" };

            foreach (var name in prefixes)
            {
                listener.Prefixes.Add(name);
            }

            listener.Start();
            Console.WriteLine("Server is listening on port 8080");

            do
            {
                var context = await listener.GetContextAsync();
                await ProcessRequest(context);
            }
            while (listener.IsListening);
        }
        private async Task ProcessRequest(HttpListenerContext context)
        {
            var request = context.Request;
            Console.WriteLine($"Connection: {request.HttpMethod} {request.RawUrl} {request.Url.AbsolutePath}");

            var response = context.Response;
            var outputStream = response.OutputStream;
            switch (request.RawUrl)
            {
                case "/":
                    {
                        string text = "Hello, World!";
                        byte[] buffer = Encoding.UTF8.GetBytes(text);
                        response.Headers.Add("Content-Type", "text/html");
                        response.ContentLength64 = buffer.Length;
                        await outputStream.WriteAsync(buffer, 0, buffer.Length);
                        outputStream.Close();
                    }
                    break;
                case "/json":
                    {
                        var product = new
                        {
                            Name = "Macbook Pro",
                            Price = 200,
                            Manufacturer = "Apple"
                        };
                        var jsonString = JsonConvert.SerializeObject(product);
                        byte[] buffer = Encoding.UTF8.GetBytes(jsonString);
                        response.Headers.Add("Content-Type", "application/json");
                        response.ContentLength64 = buffer.Length;
                        await outputStream.WriteAsync(buffer, 0, buffer.Length);
                        outputStream.Close();
                    }
                    break;
                case "/requestInfo":
                    {
                        string htmlString = GenerateHtml(request);
                        byte[] buffer = Encoding.UTF8.GetBytes(htmlString);
                        response.StatusCode = (int)HttpStatusCode.OK;
                        response.Headers.Add("Content-Type", "text/html");
                        response.ContentLength64 = buffer.Length;
                        await outputStream.WriteAsync(buffer, 0, buffer.Length);
                        outputStream.Close();
                    }
                    break;
                default:
                    {
                        string text = "Not Found!";
                        byte[] buffer = Encoding.UTF8.GetBytes(text);
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        await outputStream.WriteAsync(buffer, 0, buffer.Length);
                        outputStream.Close();
                    }
                    break;
            }
        }
        private string GenerateHtml(HttpListenerRequest request)
        {
            string format = @"<!DOCTYPE html>
                            <html lang=""en""> 
                                <head>
                                    <meta charset=""UTF-8"">
                                    {0}
                                 </head> 
                                <body>
                                    {1}
                                </body> 
                            </html>";

            string head = "<title>Test WebServer</title>";

            var body = new StringBuilder();
            body.Append("<h1>Request Info</h1>");
            body.Append("<h2>Request Header:</h2>");

            // Header information
            var headers = from key in request.Headers.AllKeys
                          select $"<div>{key} : {string.Join(",", request.Headers.GetValues(key))}</div>";
            body.Append(string.Join("", headers));

            //Extract request properties
            body.Append("<h2>Request properties:</h2>");
            var properties = request.GetType().GetProperties();
            foreach (var property in properties)
            {
                var name_pro = property.Name;
                string? value_pro;
                try
                {
                    value_pro = property.GetValue(request)?.ToString();
                }
                catch (Exception e)
                {
                    value_pro = e.Message;
                }
                body.Append($"<div>{name_pro} : {value_pro}</div>");

            }
            ;
            string html = string.Format(format, head, body.ToString());
            return html;
        }
    }
}
