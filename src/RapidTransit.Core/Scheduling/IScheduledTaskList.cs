namespace RapidTransit.Core.Scheduling
{
    using System;
    using System.Threading.Tasks;


    public interface IScheduledTaskList
    {
        /// <summary>
        /// Returns the number of scheduled tasks
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Returns an array of Tasks that are ready to be started.
        /// </summary>
        /// <param name="now"></param>
        /// <returns></returns>
        Task[] GetReadyTasks(DateTime now);

        /// <summary>
        /// Returns the time slot of the next scheduled task, to allow the thread scheduler to efficiently
        /// wait to ask for ready tasks
        /// </summary>
        /// <param name="now">The current time to start from (in UTC)</param>
        /// <param name="scheduledAt">The time slot of the next scheduled task</param>
        /// <returns>True if a task is scheduled, False if no tasks are scheduled</returns>
        bool GetNextTimeSlot(DateTime now, out DateTime scheduledAt);

        /// <summary>
        /// Add a task to the list at the specified time slot
        /// </summary>
        /// <param name="timeSlot"></param>
        /// <param name="task"></param>
        void Add(DateTime timeSlot, Task task);
    }
}