namespace Tamos.AbleOrigin.Payment.ShouQianBa
{
    internal static class SqbValidator
    {
        public static PaymentState GetPayState(this ISqbOrderInfo order)
        {
            //订单状态列表：https://doc.shouqianba.com/zh-cn/api/annex/resultCode.html
            return order.order_status switch
            {
                "CREATED" => PaymentState.WaitPaying,
                "PAID" => PaymentState.PaymentSuccess,
                
                _=> PaymentState.PayFail
            };
        }

        
        #region Response check

        internal static string GetErrorDes<T>(this Result<BizData<T>> res)
        {
            return res.biz_response == null
                ? $"{res.error_code}: {res.error_message}"
                : $"{res.biz_response.error_code}: {res.biz_response.error_message}";
        }

        #endregion
    }
}