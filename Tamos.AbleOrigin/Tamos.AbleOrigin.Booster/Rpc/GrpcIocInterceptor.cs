using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Tamos.AbleOrigin.IOC;

namespace Tamos.AbleOrigin.Booster
{
    public class GrpcIocInterceptor : Interceptor
    {
        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            using (ServiceLocator.Container.BeginScope())
            {
                //var response = await continuation(request, context);
                return await base.UnaryServerHandler(request, context, continuation);    
            }
        }
    }
}