using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Timers;

namespace GenericWindowsService.BL
{
    public class ServiceManager : IServiceManager
    {
        public List<IGenericServiceItem> ServiceItemList { get; set; }
        private readonly IServiceItemsLoader _loader;
        private readonly Timer _dailyTimer;

        public ServiceManager() : this(GetLoaderFromConfig())
        {
        }

        public ServiceManager(IServiceItemsLoader loader)
        {
            loader.ConfigFile = ConfigurationManager.AppSettings["ConfigFile"];
            FileSystemWatcher watcher = new FileSystemWatcher(loader.ConfigFile);

            _loader = loader;
            watcher.Changed += watcher_Changed;
            ServiceItemList = new List<IGenericServiceItem>();

            _dailyTimer = new Timer();
            _dailyTimer.Elapsed += DailyTimerElapsed;
        }
        
        public void Start()
        {
            InitTimerInterval();
        }

        public void Stop()
        {
            _dailyTimer.Stop();

            foreach (IGenericServiceItem genericServiceItem in ServiceItemList)
            {
                genericServiceItem.StopTimer();
            }
        }
        
        public void InitTimerInterval()
        {
            ServiceItemList = _loader.LoadAll();

            DateTime nextDay = SystemTime.Now().AddDays(1);
            double interval = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, 0, 0, 0).Subtract(SystemTime.Now()).TotalMilliseconds;
            _dailyTimer.Interval = interval;
            DailyTimerElapsed(this, null);
            _dailyTimer.Start();
        }
        
        public void SetAllServiceTimers()
        {
            foreach (IGenericServiceItem genericServiceItem in ServiceItemList)
            {
                genericServiceItem.InitTimerInterval();
            }
        }

        private void DailyTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _dailyTimer.Stop();
            SetAllServiceTimers();
            InitTimerInterval();
        }

        private void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            SetAllServiceTimers();
        }

        // helper methods
        private static IServiceItemsLoader GetLoaderFromConfig()
        {
            IServiceItemsLoader result;
            string configType = ConfigurationManager.AppSettings["ConfigType"];

            switch (configType.ToLower())
            {
                case "json":
                    result = new JsonConfigLoader();
                    break;
                case "cron":
                    result = new CronConfigLoader();
                    break;
                default:
                    result = new JsonConfigLoader();
                    break;
            }

            return result;
        }
    }
}
