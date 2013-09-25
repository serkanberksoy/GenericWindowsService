using System;
using System.Collections.Generic;
using GenericWindowsService.BL;
using NUnit.Framework;

namespace GenericWindowsService.Tests
{
    [TestFixture]
    public class ScheduleTests
    {
        [Test]
        public void RunEveryDayAtSevenTest()
        {
            IExecutionSchedule executionSchedule = new ServiceItemExecutionSchedule();

            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                executionSchedule.AddWeeklySchedule(day, new TimeSpan(7, 0, 0));
            }

            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                Assert.IsTrue(executionSchedule.GetDailySchedule(day, false, false).Contains(new TimeSpan(7, 0, 0)));
            }
        }

        [Test]
        public void RunMondayAtThreeThirtyThursdayAtNineFifteenTest()
        {
            IExecutionSchedule executionSchedule = new ServiceItemExecutionSchedule();
            executionSchedule.AddWeeklySchedule(DayOfWeek.Monday, new TimeSpan(3, 30, 0));
            executionSchedule.AddWeeklySchedule(DayOfWeek.Thursday, new TimeSpan(9, 15, 0));


            Assert.IsTrue(executionSchedule.GetDailySchedule(DayOfWeek.Monday, false, false).Contains(new TimeSpan(3, 30, 0)));
            Assert.IsTrue(executionSchedule.GetDailySchedule(DayOfWeek.Thursday, false, false).Contains(new TimeSpan(9, 15, 0)));
        }

        [Test]
        public void DontStartScheduleUntil15ThJanuary2023()
        {
            IExecutionSchedule executionSchedule = new ServiceItemExecutionSchedule();
            executionSchedule.AddWeeklySchedule(DayOfWeek.Monday, new TimeSpan(10, 0, 0));
            executionSchedule.StartDate = new DateTime(2023, 1, 15);

            Assert.AreEqual(0, executionSchedule.GetDailySchedule(DayOfWeek.Monday).Count);
        }

        [Test]
        public void EndedScheduleShouldNotReturnListItem()
        {
            IExecutionSchedule executionSchedule = new ServiceItemExecutionSchedule();
            executionSchedule.AddWeeklySchedule(DayOfWeek.Monday, new TimeSpan(10, 0, 0));
            executionSchedule.EndDate = new DateTime(2000, 1, 15);

            Assert.AreEqual(0, executionSchedule.GetDailySchedule(DayOfWeek.Monday).Count);
        }

        [Test]
        public void BetweenStartandEndDatePositiveTest()
        {
            IExecutionSchedule executionSchedule = new ServiceItemExecutionSchedule();
            executionSchedule.AddWeeklySchedule(DayOfWeek.Monday, new TimeSpan(10, 0, 0));
            executionSchedule.StartDate = new DateTime(2000, 1, 15);
            executionSchedule.EndDate = new DateTime(2023, 1, 15);

            Assert.AreEqual(1, executionSchedule.GetDailySchedule(DayOfWeek.Monday).Count);
        }

        [Test]
        public void RepeatingScheduleTest()
        {
            IExecutionSchedule executionSchedule = new ServiceItemExecutionSchedule();
            executionSchedule.StartDate = new DateTime(2000, 1, 15);
            executionSchedule.EndDate = new DateTime(2023, 1, 15);

            executionSchedule.AddWeeklyRepeatingSchedule(new List<DayOfWeek>{DayOfWeek.Monday, DayOfWeek.Wednesday}, new TimeSpan(10, 0, 0), new TimeSpan(15, 0, 0), new TimeSpan(0, 20, 0));

            List<TimeSpan> mondaySchedule = executionSchedule.GetDailySchedule(DayOfWeek.Monday);
            Assert.AreEqual(16, mondaySchedule.Count);
            CollectionAssert.Contains(mondaySchedule, new TimeSpan(10, 0, 0));
            CollectionAssert.Contains(mondaySchedule, new TimeSpan(15, 0, 0));
            CollectionAssert.Contains(mondaySchedule, new TimeSpan(11, 20, 0));

            List<TimeSpan> wednesdaySchedule = executionSchedule.GetDailySchedule(DayOfWeek.Wednesday);
            Assert.AreEqual(16, wednesdaySchedule.Count);
        
            List<TimeSpan> sundaySchedule = executionSchedule.GetDailySchedule(DayOfWeek.Sunday);
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
            double tenMinutesInMilliSeconds = 10*60*1000;
            Assert.AreEqual(tenMinutesInMilliSeconds , executionSchedule.GetNextWeeklyItemMilliseconds(DayOfWeek.Monday, date));
        }

        [Test]
        public void ExcludedSchedulesTest()
        {
            IExecutionSchedule executionSchedule = new ServiceItemExecutionSchedule();
            executionSchedule.StartDate = new DateTime(2000, 1, 15);
            executionSchedule.EndDate = new DateTime(2023, 1, 15);

            executionSchedule.AddWeeklyRepeatingSchedule(new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday }, new TimeSpan(10, 0, 0), new TimeSpan(15, 0, 0), new TimeSpan(0, 20, 0));
            executionSchedule.ExcludeWeeklySchedule(new[] {DayOfWeek.Wednesday}, TimeSpan.FromHours(12), TimeSpan.FromHours(13));

            List<TimeSpan> mondaySchedule = executionSchedule.GetDailySchedule(DayOfWeek.Monday);
            Assert.AreEqual(16, mondaySchedule.Count);
            CollectionAssert.Contains(mondaySchedule, new TimeSpan(10, 0, 0));
            
            List<TimeSpan> wednesdaySchedule = executionSchedule.GetDailySchedule(DayOfWeek.Wednesday);

            CollectionAssert.DoesNotContain(wednesdaySchedule, new TimeSpan(12, 20, 0));
            CollectionAssert.DoesNotContain(wednesdaySchedule, new TimeSpan(12, 00, 0));
            CollectionAssert.DoesNotContain(wednesdaySchedule, new TimeSpan(13, 00, 0));

            Assert.AreEqual(12, wednesdaySchedule.Count);
            CollectionAssert.Contains(wednesdaySchedule, new TimeSpan(11, 40, 0));
            CollectionAssert.Contains(wednesdaySchedule, new TimeSpan(13, 20, 0));
        }

        [Test]
        public void AddOneTimeSchedule()
        {
            SystemTime.Now = () => new DateTime(2013, 09, 25, 13, 0, 0);
            DateTime schedule1 = SystemTime.Now().AddMinutes(10);
            DateTime schedule2 = SystemTime.Now().AddDays(1).AddHours(5).AddMinutes(10);


            IExecutionSchedule executionSchedule = new ServiceItemExecutionSchedule();
            executionSchedule.StartDate = new DateTime(2000, 1, 15);
            executionSchedule.EndDate = new DateTime(2023, 1, 15);

            executionSchedule.OneTimeSchedule.AddRange(new[]{schedule1, schedule2});
            
            Assert.Greater(executionSchedule.GetNextItemMilliseconds(), 0);

            /*
             * method 1 to test time is to add execution time
            const double executeTime = 100;
            
            Assert.Greater(executionSchedule.GetNextItemMilliseconds() + executeTime, tenMinutesInMilliSeconds);
             * */

            // method 2 from ayende is static Func<DateTime> Now = () => DateTime.Now

            const double tenMinutesInMilliSeconds = 10 * 60 * 1000;
            Assert.AreEqual(executionSchedule.GetNextItemMilliseconds() , tenMinutesInMilliSeconds);

        }

        // TODO: Add monthly schedule tests and methods
        // TODO: add one time schedule tests and methods
    }
}
