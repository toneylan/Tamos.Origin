using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Tamos.AbleOrigin.AspNetCore
{
    public static class RequestExtend
    {
        #region Request 参数

        /// <summary>
        /// 获取请求参数，并转为int类型
        /// </summary>
        public static int GetQueryInt(this HttpRequest request, string paraName, int defVal = 0)
        {
            return request.Query.TryGetValue(paraName, out var val) ? Utility.StrToInt(val, defVal) : defVal;
        }

        /// <summary>
        /// 获取请求参数，并转为Long类型
        /// </summary>
        public static long GetQueryLong(this HttpRequest request, string paraName, long defVal = 0)
        {
            return request.Query.TryGetValue(paraName, out var val) ? Utility.StrToLong(val, defVal) : defVal;
        }

        /// <summary>
        /// 获取PostForm参数，并转为int类型
        /// </summary>
        public static int GetFormInt(this HttpRequest request, string paraName, int defVal = 0)
        {
            return request.Form.TryGetValue(paraName, out var val) ? Utility.StrToInt(val, defVal) : defVal;
        }

        /// <summary>
        /// 获取PostForm参数，并转为Long类型
        /// </summary>
        public static long GetFormLong(this HttpRequest request, string paraName, long defVal = 0)
        {
            return request.Form.TryGetValue(paraName, out var val) ? Utility.StrToLong(val, defVal) : defVal;
        }

        #endregion

        #region Url 解析
        
        /// <summary>
        /// 获取当前请求域名（含协议、端口），如：http://m.tamos.com:808
        /// </summary>
        /// <param name="request"></param>
        /// <param name="onlyHost">只要域名部分</param>
        /// <param name="devReturl">用于Docker开发环境时，从返回地址来解析Host。因Node devserver做代理时，当前请求域名(host.docker.internal)并不是访问域名</param>
        /// <returns></returns>
        public static string GetDomain(this HttpRequest request, bool onlyHost = false, string devReturl = null)
        {
            if (devReturl != null && CentralConfiguration.IsEnvDev()) return WebUtility.GetDomain(devReturl, onlyHost);
            
            return onlyHost ? request.Host.Host : $"{request.Scheme}://{request.Host}";
        }
        
        /// <summary>
        /// 获取顶级域名（不含端口）
        /// </summary>
        public static string GetRootDomain(this HttpRequest request)
        {
            return WebUtility.GetRootDomain(request.Host.Host);
        }
        
        /// <summary>
        /// 获取请求的完整Url，包含域名信息且过滤了代理端口
        /// </summary>
        public static string GetRequestUrl(this HttpRequest request)
        {
            return $"{request.GetDisplayUrl()}{request.QueryString}";
        }

        #endregion

        /*#region Request file

        public static HttpUploadFile ToUploadFile(this IFormFile file)
        {
            if (file == null || file.Length <= 0) return null;

            var uploadFile = new HttpUploadFile
            {
                ContentType = file.ContentType,
                FileName = file.FileName
            };
            
            using var ms = new MemoryStream();
            file.CopyTo(ms);
            uploadFile.Content = ms.ToArray();

            return uploadFile;
        }
        
        #endregion*/

        /// <summary>
        /// 尝试从Authenticate结果来获取UserId，避免重复从Passport服务（缓存）查询。
        /// </summary>
        public static long GetUserId(this ClaimsPrincipal user)
        {
            return PassportAuthHandler.GetUserId(user);
        }

        #region DumpRequest

        /// <summary>
        /// 异步读取HttpRequest Body数据。如要多次读取Body，需提前调用request.EnableBuffering()。
        /// </summary>
        public static async Task<string?> ReadBody(this HttpRequest request)
        {
            //FormContentType 时可能没有，被参数绑定读取过的原因？待测试
            // IMPORTANT: Leave the body open so the next middleware can read it.
            using var reader = new StreamReader(request.Body, Encoding.UTF8, false, leaveOpen: true);
            var bodyContent = await reader.ReadToEndAsync(); //默认不支持同步方法：ReadToEnd()
            
            //request.Body.Position = 0; //重置，备重复读取
            return bodyContent;
        }

        /// <summary>
        /// 导出Http请求的完整信息。
        /// </summary>
        public static string DumpRequest(this HttpRequest request, string? bodyContent)
        {
            try
            {
                var sb = new StringBuilder();
                void AppendCollection<T>(IEnumerable<KeyValuePair<string, T>> paras, string name)
                {
                    sb.AppendLine($"-------------  {name} --------------");
                    foreach (var item in paras)
                    {
                        AppendFeild(item.Key, item.Value);
                    }
                }

                void AppendFeild<T>(string name, T value) => sb.AppendLine(name.PadLeft(20, ' ') + " : " + value);

                //--- 基本信息
                sb.AppendLine("-------------  Client --------------");
                AppendFeild("ClientIP", WebUtility.GetClientIP(request));
                AppendFeild("HttpMethod", request.Method);

                AppendCollection(request.Headers, "Headers");
                if (request.Cookies.Count > 0) AppendCollection(request.Cookies, "Cookies");

                //--- 请求参数
                if (request.Query.Count > 0) AppendCollection(request.Query, "Query");
                if (bodyContent.NotNull()) sb.AppendLine($"-------------  Body --------------\n{bodyContent}");

                return sb.ToString();
            }
            catch (Exception e)
            {
                LogService.Error(e);
                return string.Empty;
            }
        }

        #endregion
    }
}