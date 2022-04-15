using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using Tamos.AbleOrigin;

namespace Tamos.MetaCoding.GeneratorDesign
{
    class Program
    {
        static void Main(string[] args)
        {
            //parse args
            /*if (args.Length > 0)
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
            }*/

            //-- get config
            var confPath = args.LastOrDefault();
            if (confPath.IsNull() || !File.Exists(confPath))
            {
                Console.WriteLine("[TamosGenerator] exit: config file not set or not exists.");
                return;
            }

            var basePath = Path.GetDirectoryName(confPath);
            var conf = JsonSerializer.Deserialize<CodeBuildConfig>(File.ReadAllText(confPath));
            //reset path from base path
            // ReSharper disable once PossibleNullReferenceException
            conf.AssemblyPath = Path.Combine(basePath, conf.AssemblyPath ?? "../bin/Debug/net6.0");
            conf.OutputPath = Path.Combine(basePath, conf.OutputPath ?? string.Empty);
            if (conf.OutNamespace.IsNull()) conf.OutNamespace = conf.AssemblyInterface[0];
            
            var watch = Stopwatch.StartNew();
            //grpc build
            if (conf.OutServiceName.NotNull())
            {
                try
                {
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
