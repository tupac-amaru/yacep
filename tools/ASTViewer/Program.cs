using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Swifter.Json;
using Swifter.Readers;
using Swifter.RW;
using Swifter.Writers;
using TupacAmaru.Yacep;
using TupacAmaru.Yacep.Extensions;
using TupacAmaru.Yacep.Core;
using TupacAmaru.Yacep.Symbols;

namespace ASTViewer
{
    public class Program
    {
        private class ValueInterface<T> : IValueInterface<T>
        {
            private readonly Action<T, IValueWriter> _writer;
            public ValueInterface(Action<T, IValueWriter> writer) => _writer = writer;
            public T ReadValue(IValueReader valueReader) => throw new NotImplementedException();
            public void WriteValue(IValueWriter valueWriter, T value) => _writer(value, valueWriter);
        }

        public static async Task Main(string[] args)
        {
            var formatter = new JsonFormatter();
            formatter.SetValueInterface(new ValueInterface<Type>((t, w) => w.WriteString(t.FullName)));
            formatter.SetValueInterface(new ValueInterface<BinaryOperator>((b, w) => w.WriteString($"{b.Operator}(p:{b.Precedence})")));
            formatter.SetValueInterface(new ValueInterface<NakedFunction>((n, w) => w.WriteString(n.Name)));
            formatter.SetValueInterface(new ValueInterface<UnaryOperator>((u, w) => w.WriteString(u.Operator)));

            await new WebHostBuilder()
              .ConfigureServices(services => services.AddSingleton(Parser.Default).AddSingleton(formatter))
              .UseUrls("http://localhost:5000")
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
                var formatter = context.RequestServices.GetService<JsonFormatter>();
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
                            var result = new
                            {
                                ast = expression,
                                str = expression.ToPrettyString()
                            };
                            await resp.WriteAsync(formatter.Serialize(result));
                        }
                        catch (Exception e)
                        {
                            await resp.WriteAsync(formatter.Serialize(new
                            {
                                error = e.Message
                            }));
                        }
                    }
                }
            });
        }
    }
}
