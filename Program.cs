using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebserverCSharpXTLab
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Server server = new Server();
            await server.StartServer();
        }
    }
}
