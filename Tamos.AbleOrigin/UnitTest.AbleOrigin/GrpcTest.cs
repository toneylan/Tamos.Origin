using System;
using System.IO;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Grpc.Server.AspNetCore;
using NUnit.Framework;
using ProtoBuf;
using ProtoBuf.Grpc.Client;
using Tamos.AbleOrigin.DataProto;

namespace Tamos.AbleOrigin.UnitTest
{
    public class GrpcTest
    {
        //private readonly Channel channel = new Channel("127.0.0.1", 10042, ChannelCredentials.Insecure);

        [Test]
        public void SerializeTest()
        {
            /*var obj = new TestDataObj
            {
                Size = (80, 40)
            };

            using var stream = new MemoryStream();
            Serializer.Serialize(stream, obj);

            stream.Position = 0;
            var res = Serializer.Deserialize<TestDataObj>(stream);

            Assert.IsTrue(res.Size.Width > 0);*/
        }

        [Test]
        public void CallAspNet()
        {
            using var channel2 = GrpcChannel.ForAddress("https://localhost:5001");
            var client =  new Greeter.GreeterClient(channel2);
            var reply = client.SayHelloAsync(new HelloRequest { Name = "GreeterClient" }).ResponseAsync.Result;
            Console.WriteLine("Greeting: " + reply.Message);
            
        }

        [Test]
        public async Task Call()
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;
            using var channel2 = GrpcChannel.ForAddress("http://localhost:10042");
            //var client =  channel2.CreateGrpcService<ITestService>();
                
            //First call
            /*var res = await client.GetDataAsync(new NotifyMsg());
            Console.WriteLine(res.BaseName);*/
        }

        [Test]
        public async Task AsyncCall()
        {
            /*try
            {
                //First call
                var srv = channel.CreateGrpcService<ITestService>();

                //var cancel = new CancellationTokenSource();
                var headers = new Metadata
                {
                    {"Tamos-hd", "test val"}
                };

                var options = new CallOptions(headers);

                var count = 0;
                await foreach (var msg in srv.SubscribeAsync(options))
                {
                    //if (++count > 3) cancel.Cancel();
                    Console.WriteLine($"{msg.Key}");
                    Thread.Sleep(1000);
                }

                Console.WriteLine("Client receive end.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }*/
        }
    }
}