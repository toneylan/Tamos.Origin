using System;
using Microsoft.AspNetCore.Http;
using Tamos.AbleOrigin.DataProto;

namespace Tamos.AbleOrigin.AspNetCore
{
    public sealed class PassportOptions
    {
        internal static readonly PassportOptions Current = new();

        /// <summary>
        /// Token的Cookie键值
        /// </summary>
        public string TokenCookieName { get; set; } = "tpptoken";

        //public PassportStore? StoreProvider { get; set; }

        /// <summary>
        /// Custom auth logic, Will be called in passport auth method.
        /// </summary>
        public Func<HttpContext, PassportInfo?, GeneralRes>? AuthHandle { get; set; }
    }
}