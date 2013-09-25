using System;
using System.Collections.Generic;

namespace GenericWindowsService.BL
{
    public interface IExecutionSchedule
    {
        List<DateTime> OneTimeSchedule { get; set; }
        void AddWeeklySchedule(DayOfWeek day, TimeSpan time);
        List<TimeSpan> GetDailySchedule(DayOfWeek dayOfWeek, bool checkStartDate = true, bool checkEndDate = true);
        double GetNextItemMilliseconds();
        DateTime StartDate { get; set; }
        DateTime EndDate { get; set; }
        void AddWeeklyRepeatingSchedule(List<DayOfWeek> days, TimeSpan startTime, TimeSpan endTime, TimeSpan interval);
        void ExcludeWeeklySchedule(IEnumerable<DayOfWeek> dayOfWeeks, TimeSpan startTime, TimeSpan endTime);
    }
}
