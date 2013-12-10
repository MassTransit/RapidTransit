namespace RapidTransit.Core.Scheduling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;


    public class ScheduledTaskList :
        IScheduledTaskList
    {
        readonly object _lock = new object();
        readonly SortedList<DateTime, List<Task>> _tasks;

        public ScheduledTaskList()
        {
            _tasks = new SortedList<DateTime, List<Task>>();
        }

        public int Count
        {
            get
            {
                lock (_lock)
                    return _tasks.Sum(x => x.Value.Count);
            }
        }

        /// <summary>
        /// Returns an array of Tasks that are ready to be started.
        /// </summary>
        /// <param name="now"></param>
        /// <returns></returns>
        public Task[] GetReadyTasks(DateTime now)
        {
            lock (_lock)
            {
                KeyValuePair<DateTime, List<Task>>[] ready = _tasks
                    .Where(x => x.Key <= now)
                    .OrderBy(x => x.Key)
                    .ToArray();

                foreach (var result in ready)
                {
                    if (_tasks.ContainsKey(result.Key))
                        _tasks.Remove(result.Key);
                }

                return ready.SelectMany(x => x.Value).ToArray();
            }
        }

        /// <summary>
        /// Returns the time slot of the next scheduled task, to allow the thread scheduler to efficiently
        /// wait to ask for ready tasks
        /// </summary>
        /// <param name="now">The current time to start from (in UTC)</param>
        /// <param name="scheduledAt">The time slot of the next scheduled task</param>
        /// <returns>True if a task is scheduled, False if no tasks are scheduled</returns>
        public bool GetNextTimeSlot(DateTime now, out DateTime scheduledAt)
        {
            now = now.ToUniversalTime();

            scheduledAt = now;

            lock (_lock)
            {
                if (_tasks.Count == 0)
                    return false;

                foreach (var pair in _tasks)
                {
                    if (now >= pair.Key)
                        return true;

                    scheduledAt = pair.Key;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Add a task to the list at the specified time slot
        /// </summary>
        /// <param name="timeSlot"></param>
        /// <param name="task"></param>
        public void Add(DateTime timeSlot, Task task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            timeSlot = timeSlot.ToUniversalTime();

            lock (_lock)
            {
                List<Task> list;
                if (_tasks.TryGetValue(timeSlot, out list))
                    list.Add(task);
                else
                {
                    list = new List<Task>
                        {
                            task
                        };
                    _tasks[timeSlot] = list;
                }
            }
        }
    }
}