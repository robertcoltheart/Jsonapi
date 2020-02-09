using System.Text.Json;
using JsonApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace JsonApiWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(x =>
                {
                    x.UseUrls("http://localhost:8080");
                    x.UseStartup<Startup>();
                })
                .Build()
                .Run();
        }
    }
}
