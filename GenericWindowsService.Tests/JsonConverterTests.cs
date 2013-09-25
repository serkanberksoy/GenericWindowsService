using System;
using System.Collections.Generic;
using GenericWindowsService.BL;
using NUnit.Framework;

namespace GenericWindowsService.Tests
{
    [TestFixture]
    public class JsonConverterTests
    {
        private List<JsonConfigLoader.SServiceItemConfig> _serviceConfiguration;
        private JsonConfigLoader _configLoader;

        [TestFixtureSetUp]
        public void TestSetup()
        {
            _configLoader = new JsonConfigLoader();

            _serviceConfiguration = new List<JsonConfigLoader.SServiceItemConfig>();
                
            _serviceConfiguration.Add(new JsonConfigLoader.SServiceItemConfig
            {
                AssemblyName = "abc.dll",
                AssemblyPath = @"c:\program files\my assemblies",
                StartDate = SystemTime.Now(),
                EndDate = SystemTime.Now(),
                Days = new List<JsonConfigLoader.SServiceItemConfig.SServiceItemDay>
                        {
                            new JsonConfigLoader.SServiceItemConfig.SServiceItemDay{Time = TimeSpan.Parse("11:20") , WeekDay  = DayOfWeek.Monday},
                            new JsonConfigLoader.SServiceItemConfig.SServiceItemDay{ Time = TimeSpan.Parse("11:45"), WeekDay = DayOfWeek.Thursday}
                        },
                RepeatedAdds = new List<JsonConfigLoader.SServiceItemConfig.SServiceItemRepeated>{new JsonConfigLoader.SServiceItemConfig.SServiceItemRepeated{WeekDays = new List<DayOfWeek>{DayOfWeek.Monday, DayOfWeek.Friday},
                        StartTime = TimeSpan.Parse("04:00"), EndTime = TimeSpan.Parse("05:00"), Interval = TimeSpan.Parse("30")}}
            });
        }

        [Test]
        public void SerializeTest()
        {
            var serviceConfiguration = new List<JsonConfigLoader.SServiceItemConfig>();
            Console.WriteLine(_configLoader.SerializeSampleJson(serviceConfiguration));
        }

        [Test]
        public void DeserializeTest()
        {
            List<JsonConfigLoader.SServiceItemConfig> result = _configLoader.DeserializeConfiguration(_configLoader.SerializeSampleJson(_serviceConfiguration));

            foreach (var sServiceItemConfig in result)
            {
                Console.WriteLine(sServiceItemConfig.AssemblyName);
            }
        }
    }
}
