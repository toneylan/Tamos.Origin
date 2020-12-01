using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace Tamos.MetaCoding.GeneratorDesign
{
    class Program
    {
        static void Main(string[] args)
        {
            //parse args
            var basePath = string.Empty;
            string grpcConfPath = null;
            if (args.Length > 0)
            {
                try
                {
                    for (var i = 0; i < args.Length; i++)
                    {
                        switch (args[i])
                        {
                            case "-bpath":
                                basePath = ++i < args.Length ? args[i] : string.Empty;
                                break;
                            case "-grpc":
                                grpcConfPath = ++i < args.Length ? args[i] : null;
                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            var watch = Stopwatch.StartNew();
            //grpc build
            if (!string.IsNullOrEmpty(grpcConfPath))
            {
                try
                {
                    //var conf = JsonConvert.DeserializeObject<GrpcBuildConfig>(File.ReadAllText(Path.Combine(basePath, grpcConfPath)));
                    var conf = JsonSerializer.Deserialize<GrpcBuildConfig>(File.ReadAllText(Path.Combine(basePath, grpcConfPath)));
                    //reset path from base path
                    conf.OutputPath = Path.Combine(basePath, conf.OutputPath);
                    conf.AssemblyPath = Path.Combine(basePath, conf.AssemblyPath);
                    new GrpcServiceBuilder(conf).BuildServiceCode().Wait();

                    watch.Stop();
                    Console.WriteLine("[TamosGenerator] complete:{0}, use time:{1}ms", conf.OutServiceName, watch.ElapsedMilliseconds);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Generator error: " + ex);
                }
            }
        }
    }
}
