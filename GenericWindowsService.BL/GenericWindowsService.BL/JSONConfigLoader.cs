using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace GenericWindowsService.BL
{
    public class JsonConfigLoader : IServiceItemsLoader
    {

        public string ConfigFile { get; set; }
        public List<SServiceItemConfig> ServiceConfiguration { get; set; }

        public struct SServiceItemConfig
        {
            public string AssemblyName { get; set; }
            public string AssemblyPath { get; set; }
            public string MethodName { get; set; }
            public string FullyQualifiedClassName { get; set; }

            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }

            public List<SServiceItemDay> Days { get; set; }
            public List<SServiceItemRepeated> RepeatedAdds { get; set; }
            public List<SServiceItemExcluded> ExcludedAdds { get; set; }

            public struct SServiceItemDay
            {
                public DayOfWeek WeekDay { get; set; }
                public TimeSpan Time { get; set; }
            }

            public struct SServiceItemRepeated
            {
                public List<DayOfWeek> WeekDays { get; set; }
                public TimeSpan StartTime { get; set; }
                public TimeSpan EndTime { get; set; }
                public TimeSpan Interval{ get; set; }
            }
            public struct SServiceItemExcluded
            {
                public List<DayOfWeek> WeekDays { get; set; }
                public TimeSpan StartTime { get; set; }
                public TimeSpan EndTime { get; set; }
            }
        }

        public List<IGenericServiceItem> LoadAll()
        {
            List<IGenericServiceItem> result = new List<IGenericServiceItem>();

            if (!string.IsNullOrEmpty(ConfigFile))
            {
                var fullPath = Path.Combine(Environment.CurrentDirectory, ConfigFile);
                if (File.Exists(fullPath))
                {
                    List<SServiceItemConfig> configItemList = DeserializeConfiguration(File.ReadAllText(fullPath));

                    foreach (SServiceItemConfig configItem in configItemList)
                    {
                        IGenericServiceItem serviceItem = CreateServiceItemFromJsonStruct(configItem);

                        foreach (SServiceItemConfig.SServiceItemDay scheduleItem in configItem.Days)
                        {
                            serviceItem.ExecutionSchedule.AddWeeklySchedule(scheduleItem.WeekDay, scheduleItem.Time);
                        }

                        foreach (SServiceItemConfig.SServiceItemRepeated repeatedItem in configItem.RepeatedAdds)
                        {
                            serviceItem.ExecutionSchedule.AddWeeklyRepeatingSchedule(repeatedItem.WeekDays, repeatedItem.StartTime, repeatedItem.EndTime, repeatedItem.Interval);
                        }
                        foreach (SServiceItemConfig.SServiceItemExcluded excludedItem in configItem.ExcludedAdds)
                        {
                            serviceItem.ExecutionSchedule.ExcludeWeeklySchedule(excludedItem.WeekDays, excludedItem.StartTime, excludedItem.EndTime);
                        }
                    }
                }
                else
                {
                    throw new FileNotFoundException("Config file not found");
                }
            }
            else
            {
                throw new ArgumentException("Config file parameter invalid");
            }

            return result;
        }

        private IGenericServiceItem CreateServiceItemFromJsonStruct(SServiceItemConfig configItem)
        {
            return new GenericServiceItem
                       {
                           ExecutableDefinition = new ServiceItemExecutableDefinition
                                                      {
                                                          DllName = configItem.AssemblyName,
                                                          Path = configItem.AssemblyPath,
                                                          MethodName = configItem.MethodName,
                                                          FullyQualifiedClassName = configItem.FullyQualifiedClassName
                                                      },
                           ExecutionSchedule = new ServiceItemExecutionSchedule
                                                   {
                                                       StartDate = configItem.StartDate,
                                                       EndDate = configItem.EndDate
                                                   }
                       };
        }


        public string SerializeSampleJson(List<SServiceItemConfig> serviceConfiguration)
        {
            ServiceConfiguration = serviceConfiguration;
            return JsonConvert.SerializeObject(ServiceConfiguration);
        }

        public List<SServiceItemConfig> DeserializeConfiguration(string data)
        {
            return JsonConvert.DeserializeObject<List<SServiceItemConfig>>(data);
        }
    }

}