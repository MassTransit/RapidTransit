namespace RapidTransit.Core.Scheduling
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;


    [DebuggerDisplay("{GetType().Name} ( Count: {Count}, Next: {NextActionTime} )")]
    public class TimerScheduler :
        IScheduler,
        IDisposable
    {
        readonly object _lock = new object();
        readonly TimeSpan _noPeriod = TimeSpan.FromMilliseconds(-1);
        readonly IScheduledTaskList _tasks = new ScheduledTaskList();
        bool _disposed;
        bool _stopped;
        Timer _timer;

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected int Count
        {
            get { return _tasks.Count; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected string NextActionTime
        {
            get
            {
                DateTime scheduledAt;
                if (_tasks.GetNextTimeSlot(GetCurrentTime(), out scheduledAt))
                    return scheduledAt.ToString();

                return "None";
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            StopTimer();

            _disposed = true;
        }

        public void Schedule(TimeSpan interval, Task task)
        {
            Schedule(GetScheduledTime(interval), task);
        }

        public void Schedule(DateTime timeSlot, Task task)
        {
            lock (_lock)
            {
                _tasks.Add(timeSlot, task);

                StartReadyTasks();
            }
        }

        static DateTime GetCurrentTime()
        {
            return DateTime.UtcNow;
        }

        void StopTimer()
        {
            lock (_lock)
            {
                if (_timer != null)
                {
                    _timer.Dispose();
                    _timer = null;
                }

                _stopped = true;
            }
        }

        void StartReadyTasks()
        {
            lock (_lock)
            {
                if (_stopped)
                    return;

                Task[] readyTasks;
                while ((readyTasks = _tasks.GetReadyTasks(GetCurrentTime())).Length > 0)
                {
                    foreach (Task task in readyTasks)
                        task.Start();
                }

                ScheduleTimer();
            }
        }

        void ScheduleTimer()
        {
            DateTime now = GetCurrentTime();

            DateTime scheduledAt;
            if (_tasks.GetNextTimeSlot(now, out scheduledAt))
            {
                lock (_lock)
                {
                    TimeSpan dueTime = scheduledAt - now;

                    if (_timer != null)
                        _timer.Change(dueTime, _noPeriod);
                    else
                        _timer = new Timer(x => Task.Factory.StartNew(StartReadyTasks), this, dueTime, _noPeriod);
                }
            }
        }

        static DateTime GetScheduledTime(TimeSpan interval)
        {
            return GetCurrentTime() + interval;
        }
    }
}