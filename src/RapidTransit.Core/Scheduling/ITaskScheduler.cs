namespace RapidTransit.Core.Scheduling
{
    using System;
    using System.Threading.Tasks;


    public interface ITaskScheduler
    {
        void Schedule(TimeSpan interval, Task task);
        void Schedule(DateTime timeSlot, Task task);
    }
}