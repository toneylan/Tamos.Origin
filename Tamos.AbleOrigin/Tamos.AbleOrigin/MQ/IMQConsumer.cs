
namespace Tamos.AbleOrigin
{
    public interface IMQConsumer
    {
        /// <summary>
        /// 失败时最大重试次数(默认：3)，超过则丢入（死信）。
        /// </summary>
        int MaxRetry { get; set; }
    }

    public enum MQHandleRes
    {
        Success = 1,
        
        /// <summary>
        /// 失败，延迟（默认10分钟）后重试。延迟时间后会重新投递进队列。
        /// </summary>
        FailRetry = 3,

        /// <summary>
        /// 失败，丢弃消息。
        /// </summary>
        FailReject = 9
    }
}