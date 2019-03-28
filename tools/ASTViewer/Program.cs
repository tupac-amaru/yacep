using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.IO;
using System.Threading.Tasks;
using TupacAmaru.Yacep;
using TupacAmaru.Yacep.Core;
using TupacAmaru.Yacep.Extensions;

namespace ASTViewer
{
    public class Program
    {

        public static async Task Main(string[] args)
        {
            await new WebHostBuilder()
              .ConfigureServices(services => services.AddSingleton(Parser.Default))
              .UseUrls("http://*:5000")
              .Configure(Route)
              .UseKestrel()
              .Build()
              .RunAsync();
        }

        private static void Route(IApplicationBuilder app)
        {
            var dir = Path.Combine(AppContext.BaseDirectory, "www/dist");
            app.UseStaticFiles(new StaticFileOptions { FileProvider = new PhysicalFileProvider(dir) });
            app.Run(async context =>
            {
                var parser = context.RequestServices.GetService<IParser>();
                var req = context.Request;
                var resp = context.Response;
                var method = req.Method;
                if (!string.Equals(method, "POST") || !req.Path.StartsWithSegments("/api"))
                {
                    resp.Redirect("/index.html", true);
                }
                else
                {
                    resp.ContentType = "application/json";
                    using (var reader = new StreamReader(req.Body))
                    {
                        var expr = reader.ReadToEnd();
                        try
                        {
                            var expression = parser.Parse(expr);
                            await resp.WriteAsync(expression.ToPrettyString());
                        }
                        catch (Exception e)
                        {
                            await resp.WriteAsync(e.Message);
                        }
                    }
                }
            });
        }
    }
}
