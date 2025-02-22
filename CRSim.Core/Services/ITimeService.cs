using System;
using System.Timers;

namespace CRSim.Core.Services
{
    public interface ITimeService
    {
        event EventHandler OneSecondElapsed;  // 1秒触发的事件
        event EventHandler RefreshSecondsElapsed;  // 20秒触发的事件

        void Start();
        void Stop();

        DateTime GetDateTimeNow();
    }
}