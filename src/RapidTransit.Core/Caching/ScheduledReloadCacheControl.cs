namespace RapidTransit.Core.Caching
{
    using System;
    using System.Threading.Tasks;
    using Scheduling;


    /// <summary>
    /// Includes a scheduled cache reload as part of a event-based reload, so that the cache
    /// can be regularly refreshed in case a change is made directly in the database
    /// </summary>
    /// <typeparam name="TReload">The reload event type</typeparam>
    /// <typeparam name="TReloaded"></typeparam>
    public class ScheduledReloadCacheControl<TReload, TReloaded> :
        ICacheControl<TReload, TReloaded>
        where TReloaded : class
    {
        readonly ICacheControl<TReload, TReloaded> _cacheControl;
        readonly TimeSpan _cacheTimeout;
        readonly object _lock = new object();
        readonly Func<TReload> _reloadEventFactory;
        readonly IScheduler _scheduler;
        Task _updateTask;

        public ScheduledReloadCacheControl(ICacheControl<TReload, TReloaded> cacheControl,
            IScheduler scheduler,
            Func<TReload> reloadEventFactory, int cacheTimeoutInMinutes)
        {
            _reloadEventFactory = reloadEventFactory;
            _scheduler = scheduler;
            _cacheControl = cacheControl;
            _cacheTimeout = TimeSpan.FromMinutes(cacheTimeoutInMinutes);
            ScheduleNextUpdate();
        }

        public void Reload(TReload reload, out TReloaded reloaded)
        {
            lock (_lock)
            {
                ReloadCache(out reloaded);

                ScheduleNextUpdate();
            }
        }

        void ScheduleNextUpdate()
        {
            if (_updateTask == null)
            {
                var task = new Task(ScheduledUpdate);
                _scheduler.Schedule(_cacheTimeout, task);
                _updateTask = task;
            }
        }

        void ReloadCache(out TReloaded reloaded)
        {
            _cacheControl.Reload(_reloadEventFactory(), out reloaded);
        }

        void ScheduledUpdate()
        {
            lock (_lock)
            {
                _updateTask = null;

                try
                {
                    TReloaded reloaded;
                    ReloadCache(out reloaded);
                }
                finally
                {
                    ScheduleNextUpdate();
                }
            }
        }
    }


    /// <summary>
    /// Reload for a cache that supports both update and reload
    /// </summary>
    /// <typeparam name="TReload"></typeparam>
    /// <typeparam name="TUpdate"></typeparam>
    /// <typeparam name="TReloaded"></typeparam>
    /// <typeparam name="TUpdated"></typeparam>
    public class ScheduledReloadCacheControl<TReload, TReloaded, TUpdate, TUpdated> :
        ICacheControl<TReload, TReloaded, TUpdate, TUpdated>
        where TReloaded : class
    {
        readonly ICacheControl<TReload, TReloaded, TUpdate, TUpdated> _cacheControl;
        readonly object _lock = new object();
        readonly ICacheControl<TReload, TReloaded> _scheduledCacheControl;

        public ScheduledReloadCacheControl(ICacheControl<TReload, TReloaded, TUpdate, TUpdated> cacheControl,
            IScheduler scheduler, Func<TReload> reloadEventFactory, int cacheTimeoutInMinutes)
        {
            _cacheControl = cacheControl;
            _scheduledCacheControl = new ScheduledReloadCacheControl<TReload, TReloaded>(cacheControl, scheduler,
                reloadEventFactory,
                cacheTimeoutInMinutes);
        }

        public void Reload(TReload reload, out TReloaded reloaded)
        {
            lock (_lock)
                _scheduledCacheControl.Reload(reload, out reloaded);
        }

        public void Update(TUpdate update, out TUpdated updated)
        {
            lock (_lock)
                _cacheControl.Update(update, out updated);
        }
    }
}