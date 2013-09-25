using System;
using System.Collections.Generic;
using GenericWindowsService.BL;
using NUnit.Framework;

namespace GenericWindowsService.Tests
{
    [TestFixture]
    public class ScheduleTests
    {
        IExecutionSchedule _executionSchedule = new ServiceItemExecutionSchedule();
        const double ONEMINUTEINMS = 1 * 60 * 1000;
        const double FIVEMINUTEINMS = 5 * 60 * 1000;
        const double TENMINUTEINMS = 10 * 60 * 1000; 
        const double TWENTYMINUTEINMS = 20 * 60 * 1000;
        const double FORTYMINUTEINMS = 40 * 60 * 1000;

        [SetUp]
        public void TestSetup()
        {
            _executionSchedule = new ServiceItemExecutionSchedule();
        }

        [Test]
        public void RunEveryDayAtSevenTest()
        {
            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                _executionSchedule.AddWeeklySchedule(day, new TimeSpan(7, 0, 0));
            }

            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                Assert.IsTrue(_executionSchedule.GetDailySchedule(day, false, false).Contains(new TimeSpan(7, 0, 0)));
            }
        }

        [Test]
        public void RunMondayAtThreeThirtyThursdayAtNineFifteenTest()
        {
            _executionSchedule.AddWeeklySchedule(DayOfWeek.Monday, new TimeSpan(3, 30, 0));
            _executionSchedule.AddWeeklySchedule(DayOfWeek.Thursday, new TimeSpan(9, 15, 0));


            Assert.IsTrue(_executionSchedule.GetDailySchedule(DayOfWeek.Monday, false, false).Contains(new TimeSpan(3, 30, 0)));
            Assert.IsTrue(_executionSchedule.GetDailySchedule(DayOfWeek.Thursday, false, false).Contains(new TimeSpan(9, 15, 0)));
        }

        [Test]
        public void DontStartScheduleUntil15ThJanuary2023Test()
        {   
            _executionSchedule.AddWeeklySchedule(DayOfWeek.Monday, new TimeSpan(10, 0, 0));
            _executionSchedule.StartDate = new DateTime(2023, 1, 15);

            Assert.AreEqual(0, _executionSchedule.GetDailySchedule(DayOfWeek.Monday).Count);
        }

        [Test]
        public void EndedScheduleShouldNotReturnListItemTest()
        {
            _executionSchedule.AddWeeklySchedule(DayOfWeek.Monday, new TimeSpan(10, 0, 0));
            _executionSchedule.EndDate = new DateTime(2000, 1, 15);

            Assert.AreEqual(0, _executionSchedule.GetDailySchedule(DayOfWeek.Monday).Count);
        }

        [Test]
        public void BetweenStartandEndDatePositiveTest()
        {
            
            _executionSchedule.AddWeeklySchedule(DayOfWeek.Monday, new TimeSpan(10, 0, 0));
            _executionSchedule.StartDate = new DateTime(2000, 1, 15);
            _executionSchedule.EndDate = new DateTime(2023, 1, 15);

            Assert.AreEqual(1, _executionSchedule.GetDailySchedule(DayOfWeek.Monday).Count);
        }

        [Test]
        public void RepeatingScheduleTest()
        {
            _executionSchedule.StartDate = new DateTime(2000, 1, 15);
            _executionSchedule.EndDate = new DateTime(2023, 1, 15);

            _executionSchedule.AddWeeklyRepeatingSchedule(new List<DayOfWeek>{DayOfWeek.Monday, DayOfWeek.Wednesday}, new TimeSpan(10, 0, 0), new TimeSpan(15, 0, 0), new TimeSpan(0, 20, 0));

            List<TimeSpan> mondaySchedule = _executionSchedule.GetDailySchedule(DayOfWeek.Monday);
            Assert.AreEqual(16, mondaySchedule.Count);
            CollectionAssert.Contains(mondaySchedule, new TimeSpan(10, 0, 0));
            CollectionAssert.Contains(mondaySchedule, new TimeSpan(15, 0, 0));
            CollectionAssert.Contains(mondaySchedule, new TimeSpan(11, 20, 0));

            List<TimeSpan> wednesdaySchedule = _executionSchedule.GetDailySchedule(DayOfWeek.Wednesday);
            Assert.AreEqual(16, wednesdaySchedule.Count);
        
            List<TimeSpan> sundaySchedule = _executionSchedule.GetDailySchedule(DayOfWeek.Sunday);
            Assert.AreEqual(0, sundaySchedule.Count);
        }

        [Test]
        public void NextItemMillisecondsOfTheDayTest()
        {
            ServiceItemExecutionSchedule executionSchedule = new ServiceItemExecutionSchedule();
            executionSchedule.StartDate = new DateTime(2000, 1, 15);
            executionSchedule.EndDate = new DateTime(2023, 1, 15);

            executionSchedule.AddWeeklyRepeatingSchedule(new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday }, new TimeSpan(10, 0, 0), new TimeSpan(15, 0, 0), new TimeSpan(0, 20, 0));

            List<TimeSpan> mondaySchedule = executionSchedule.GetDailySchedule(DayOfWeek.Monday);
            Assert.AreEqual(16, mondaySchedule.Count);
            CollectionAssert.Contains(mondaySchedule, new TimeSpan(10, 0, 0));
            CollectionAssert.Contains(mondaySchedule, new TimeSpan(15, 0, 0));
            CollectionAssert.Contains(mondaySchedule, new TimeSpan(11, 20, 0));

            DateTime date = new DateTime(2013, 9, 23, 13, 10, 0);

            // 10 minutes to 13:20 (next schedule)
            Assert.AreEqual(TENMINUTEINMS, executionSchedule.GetNextWeeklyItemMilliseconds(DayOfWeek.Monday, date));
        }

        [Test]
        public void ExcludedSchedulesTest()
        {
            _executionSchedule.StartDate = new DateTime(2000, 1, 15);
            _executionSchedule.EndDate = new DateTime(2023, 1, 15);

            _executionSchedule.AddWeeklyRepeatingSchedule(new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday }, new TimeSpan(10, 0, 0), new TimeSpan(15, 0, 0), new TimeSpan(0, 20, 0));
            _executionSchedule.ExcludeWeeklySchedule(new[] {DayOfWeek.Wednesday}, TimeSpan.FromHours(12), TimeSpan.FromHours(13));

            List<TimeSpan> mondaySchedule = _executionSchedule.GetDailySchedule(DayOfWeek.Monday);
            Assert.AreEqual(16, mondaySchedule.Count);
            CollectionAssert.Contains(mondaySchedule, new TimeSpan(10, 0, 0));
            
            List<TimeSpan> wednesdaySchedule = _executionSchedule.GetDailySchedule(DayOfWeek.Wednesday);

            CollectionAssert.DoesNotContain(wednesdaySchedule, new TimeSpan(12, 20, 0));
            CollectionAssert.DoesNotContain(wednesdaySchedule, new TimeSpan(12, 00, 0));
            CollectionAssert.DoesNotContain(wednesdaySchedule, new TimeSpan(13, 00, 0));

            Assert.AreEqual(12, wednesdaySchedule.Count);
            CollectionAssert.Contains(wednesdaySchedule, new TimeSpan(11, 40, 0));
            CollectionAssert.Contains(wednesdaySchedule, new TimeSpan(13, 20, 0));
        }

        [Test]
        public void AddOneTimeScheduleTest()
        {
            SystemTime.Now = () => new DateTime(2013, 09, 25, 13, 0, 0);
            DateTime schedule1 = SystemTime.Now().AddMinutes(10);
            DateTime schedule2 = SystemTime.Now().AddDays(1).AddHours(5).AddMinutes(10);
            
            
            _executionSchedule.StartDate = new DateTime(2000, 1, 15);
            _executionSchedule.EndDate = new DateTime(2023, 1, 15);

            _executionSchedule.OneTimeSchedule.AddRange(new[]{schedule1, schedule2});
            
            Assert.Greater(_executionSchedule.GetNextItemMilliseconds(), 0);

            /*
             * method 1 to test time is to add execution time
            const double executeTime = 100;
            
            Assert.Greater(_executionSchedule.GetNextItemMilliseconds() + executeTime, tenMinutesInMilliSeconds);
             * */

            // method 2 from ayende is static Func<DateTime> Now = () => DateTime.Now
            
            Assert.AreEqual(_executionSchedule.GetNextItemMilliseconds() , TENMINUTEINMS);
        }
        
        [Test]
        public void GetMinimumDateForPassedOneTimeScheduleTimesTest()
        {
            SystemTime.Now = () => new DateTime(2013, 09, 25, 13, 0, 0);
            DateTime schedule1 = SystemTime.Now().AddMinutes(-10);
            DateTime schedule2 = SystemTime.Now().AddMinutes(20);
            
            _executionSchedule.StartDate = new DateTime(2000, 1, 15);
            _executionSchedule.EndDate = new DateTime(2023, 1, 15);

            _executionSchedule.OneTimeSchedule.AddRange(new[] { schedule1, schedule2 });

            Assert.Greater(_executionSchedule.GetNextItemMilliseconds(), 0);
            Assert.AreEqual(_executionSchedule.GetNextItemMilliseconds(), TWENTYMINUTEINMS);
        }

        [Test]
        public void OneTimeScheduleShouldBeBetweenStartAndEndDateTest()
        {
            SystemTime.Now = () => new DateTime(2013, 09, 25, 13, 0, 0);
            DateTime schedule1 = SystemTime.Now().AddMinutes(40);
            DateTime schedule2 = SystemTime.Now().AddMinutes(20);

            
            _executionSchedule.StartDate = new DateTime(2013, 09, 25, 13, 25, 0);
            _executionSchedule.EndDate = new DateTime(2013, 09, 25, 13, 45, 0);

            _executionSchedule.OneTimeSchedule.AddRange(new[] { schedule1, schedule2 });

            double expectedNextItemTicks = _executionSchedule.GetNextItemMilliseconds();

            Assert.Greater(expectedNextItemTicks, 0);
            Assert.AreEqual(expectedNextItemTicks, FORTYMINUTEINMS);
        }

        // Monthly schedule tests and methods
        [Test]
        public void RunEveryFirstDayofMonthTest()
        {
            TimeSpan dayFirstOfMonth = new TimeSpan(1, 15, 0, 0);

            for (byte month = 1; month < 13; month++)
            {
                _executionSchedule.AddMonthlySchedule(month, dayFirstOfMonth);
            }

            for (byte month = 1; month < 13; month++)
            {
                CollectionAssert.Contains(_executionSchedule.GetMonthlySchedule(month, false, false), dayFirstOfMonth);
            }
        }

        [Test]
        public void MonthScheduleShouldBeBetweenStartAndEndDateTest()
        {
            _executionSchedule.AddMonthlySchedule(3, new TimeSpan(10, 0, 0, 0));
            _executionSchedule.AddMonthlySchedule(3, new TimeSpan(11, 17, 0, 0));
            _executionSchedule.StartDate = new DateTime(2013, 3, 11, 0, 0, 0);
            SystemTime.Now = () => new DateTime(2013, 3, 5, 10, 0, 0, 0);

            List<TimeSpan> expectedSchedule = _executionSchedule.GetMonthlySchedule(3, true, false);
            Assert.AreEqual(1, expectedSchedule.Count);
            CollectionAssert.Contains(expectedSchedule, new TimeSpan(11, 17, 0, 0));
        }

        [Test]
        public void GetNextScheduleItemMillisecondsWithComplexScheduleTest()
        {
            SystemTime.Now = () => new DateTime(2013, 10, 1, 12, 0, 0, 0);
            _executionSchedule.OneTimeSchedule.AddRange(new[]{SystemTime.Now().AddMinutes(10)});
            
            Assert.AreEqual(TENMINUTEINMS, _executionSchedule.GetNextItemMilliseconds());

            _executionSchedule.AddWeeklySchedule(SystemTime.Now().DayOfWeek, new TimeSpan(12, 5, 0));
            Assert.AreEqual(FIVEMINUTEINMS, _executionSchedule.GetNextItemMilliseconds());

            _executionSchedule.AddMonthlySchedule(10, new TimeSpan(1, 12, 01, 0));
            Assert.AreEqual(ONEMINUTEINMS, _executionSchedule.GetNextItemMilliseconds());
        }
    }
}
