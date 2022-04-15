using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tamos.AbleOrigin
{
    /// <summary>
    /// 异步的延迟任务，保持单线程执行，可避免驻留Task来轮询或挂起。
    /// </summary>
    public class LazyTask
    {
        private int _scheduleFlag;
        private int _expectTimes; //执行任务前的计划次数
        //private Task _workTask;
        //private DateTime _scheduleRunTaskTime; //计划在什么时间点执行任务
        
        #region Property & ctor

        /// <summary>
        /// 最大延迟多少时间执行。触发Schedule后修改，下一轮Schedule才会生效。
        /// </summary>
        public TimeSpan MaxDelayTime { get; set; }

        /// <summary>
        /// 将触发执行的Action
        /// </summary>
        private Action TaskAction { get; }

        /// <summary>
        /// 是否处于计划中，但未开始此次执行计划（不代表当前没有任务执行，因为可能正在上一轮执行中）
        /// </summary>
        public bool InSchedule => _expectTimes > 0;
        
        public LazyTask(TimeSpan maxDelay, Action action)
        {
            MaxDelayTime = maxDelay;
            TaskAction = action;
        }

        #endregion

        /// <summary>
        /// 发起执行计划，支持并发调用
        /// </summary>
        public void Schedule()
        {
            //任务执行时会归零，故这里始终递增，可使正执行任务完成后，再次发起执行，以避免遗漏Schedule前的（外部应用）数据操作。
            _expectTimes++; //Interlocked.Increment(ref _expectTimes);
            if (_scheduleFlag > 0) return;

            //_scheduleRunTaskTime = DateTime.Now.AddSeconds(DelaySeconds); //DelaySeconds后再执行
            StartTask();
        }

        #region Schedule Task

        //确保单线程开启任务
        private void StartTask()
        {
            //并发限制
            if (Interlocked.CompareExchange(ref _scheduleFlag, 1, 0) > 0) return;

            //检测之前任务是否异常无法结束
            /*if (_workTask != null && _expectTimes >= 50)
            {
                LogService.ErrorFormat("LazyTask 启动异常 Schedule:{0}, WorkTask:{1}, Exp:{2} ", _expectTimes, _workTask.Status, _workTask.Exception);
            }*/
            
            //开始延迟任务
            Task.Run(TaskScheduler);
        }

        private async Task TaskScheduler()
        {
            try
            {
                //====计划时间检查
                /*while (_scheduleCount <= 3 && _scheduleRunTaskTime > DateTime.Now) //_scheduleCount防止频繁schedule时过久不执行
                {
                    var waitSpan = (int) (_scheduleRunTaskTime - DateTime.Now).TotalMilliseconds;
                    if (waitSpan > 0) Thread.Sleep(waitSpan);
                }*/

                await Task.Delay(MaxDelayTime);

                //====执行计划任务
                _expectTimes = 0; //Interlocked.Exchange(ref _expectTimes, 0);
                TaskAction();
            }
            catch (Exception e)
            {
                LogService.Error(e);
            }
            finally
            {
                Interlocked.Exchange(ref _scheduleFlag, 0);
            }

            //执行期间又有新计划，则再次发起任务
            if (_expectTimes > 0) StartTask();
        }

        #endregion
    }
}