using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebserverCSharpXTLab
{
    internal static class SampleServer
    {
        public static async Task CreateServer()
        {
            if (!HttpListener.IsSupported)
            {
                throw new Exception("Http listener is not supported");
            }

            HttpListener httpListener = new HttpListener();
            httpListener.Prefixes.Add("http://*:8080/");
            httpListener.Start();
            Console.WriteLine("Server is listening on port 8080!");

            do
            {
                var context = await httpListener.GetContextAsync();
                var request = context.Request;
                Console.WriteLine($"Connection : {request.Url.AbsolutePath} {request.HttpMethod} {request.RawUrl}");

                var response = context.Response;
                var outputStream = response.OutputStream;

                response.Headers.Add("Content-Type", "text/html");
                string text = "Xin Chao Cac Ban";
                byte[] buffer = Encoding.UTF8.GetBytes(text);
                response.ContentLength64 = buffer.Length;
                await outputStream.WriteAsync(buffer, 0, buffer.Length);
                outputStream.Close();


            } while (httpListener.IsListening);
        }
    }
}
