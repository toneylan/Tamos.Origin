using System;

namespace Tamos.AbleOrigin.Payment.WxPay
{
    internal class WxPayException : Exception 
    {
        public WxPayException(string msg) : base(msg) 
        {

        }
     }
}