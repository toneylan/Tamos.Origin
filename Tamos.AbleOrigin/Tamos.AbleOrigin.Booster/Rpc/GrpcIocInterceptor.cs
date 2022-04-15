using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Tamos.AbleOrigin.Booster
{
    //未使用，目前用的AspNetCore的Request scope，同时支持Web和Grpc
    internal class GrpcIocInterceptor : Interceptor
    {
        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            using (ServiceLocator.Container!.BeginScope())
            {
                //var response = await continuation(request, context);
                return await base.UnaryServerHandler(request, context, continuation);    
            }
        }
    }
}