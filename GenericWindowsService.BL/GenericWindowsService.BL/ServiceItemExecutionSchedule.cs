using System;
using System.Collections.Generic;
using System.Linq;

namespace GenericWindowsService.BL
{
    public class ServiceItemExecutionSchedule : IExecutionSchedule
    {
        private readonly Dictionary<DayOfWeek, List<TimeSpan>> _weeklySchedule;
        private readonly Dictionary<byte, List<TimeSpan>> _monthlySchedule;

        public List<DateTime> OneTimeSchedule { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public ServiceItemExecutionSchedule()
        {
            OneTimeSchedule = new List<DateTime>();
            _weeklySchedule = new Dictionary<DayOfWeek, List<TimeSpan>>();
            _monthlySchedule = new Dictionary<byte, List<TimeSpan>>();

            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                _weeklySchedule.Add(day, new List<TimeSpan>());
            }

            for (byte month = 0; month < 13; month++)
            {
                _monthlySchedule[month] = new List<TimeSpan>();
            }
        }

        public void AddWeeklySchedule(DayOfWeek day, TimeSpan time)
        {
            _weeklySchedule[day].Add(time);
        }

        public void AddWeeklyRepeatingSchedule(List<DayOfWeek> days, TimeSpan startTime, TimeSpan endTime, TimeSpan interval)
        {
            foreach (DayOfWeek dayOfWeek in days)
            {
                TimeSpan time = startTime;

                do
                {
                    AddWeeklySchedule(dayOfWeek, time);
                    time += interval;
                } while (time <= endTime);
            }
        }

        public void ExcludeWeeklySchedule(IEnumerable<DayOfWeek> dayOfWeeks, TimeSpan startTime, TimeSpan endTime)
        {
            var itemsToRemove = (from e in _weeklySchedule
                                 from d in dayOfWeeks
                                 where d == e.Key
                                 select e).ToDictionary(x => x.Key, x => x.Value.Where(t => t >= startTime && t <= endTime).ToList());

            foreach (var pair in itemsToRemove)
            {
                foreach (TimeSpan time in itemsToRemove[pair.Key])
                {
                    _weeklySchedule[pair.Key].Remove(time);
                }
            }
        }

        public void AddMonthlySchedule(byte month, TimeSpan time)
        {
            _monthlySchedule[month].Add(time);
        }

        public List<TimeSpan> GetDailySchedule(DayOfWeek dayOfWeek, bool checkStartDate = true, bool checkEndDate = true)
        {
            List<TimeSpan> result = new List<TimeSpan>();

            bool dateCheck = true;
            dateCheck = checkStartDate ? CheckStartDate() : true;
            dateCheck = checkEndDate && dateCheck ? CheckEndDate() : false;

            if (checkStartDate || checkEndDate)
            {
                if (dateCheck)
                {
                    result = _weeklySchedule[dayOfWeek];
                }
            }
            else
            {
                result = _weeklySchedule[dayOfWeek];
            }

            return result;
        }
        public List<TimeSpan> GetMonthlySchedule(byte month, bool checkStartDate = true, bool checkEndDate = true)
        {
            List<TimeSpan> result = new List<TimeSpan>();

            foreach (TimeSpan time in _monthlySchedule[month])
            {
                DateTime currentTime = new DateTime(SystemTime.Now().Year, month, time.Days, time.Hours, time.Minutes, time.Seconds, time.Milliseconds);

                if (checkStartDate || checkEndDate)
                {
                    if (checkStartDate)
                    {
                        if(currentTime >= StartDate)
                        {
                            if (checkEndDate)
                            {
                                if (currentTime <= EndDate)
                                {
                                    result.Add(time);
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                result.Add(time);
                            }    
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (checkEndDate && currentTime <= EndDate)
                    {
                        result.Add(time);
                    }
                }
                else
                {
                    result.Add(time);
                }
            }

            return result;
        }

        private bool CheckEndDate()
        {
            return EndDate == default(DateTime) || SystemTime.Now() <= EndDate;
        }
        private bool CheckStartDate()
        {
            return StartDate == default(DateTime) || SystemTime.Now() >= StartDate;
        }

        public double GetNextOneTimeItemInMilliseconds()
        {
            double result = default(double);

            if (OneTimeSchedule.Count > 0)
            {
                result = (from date in OneTimeSchedule
                          where date > SystemTime.Now() &&
                          (StartDate == default(DateTime) || date > StartDate) &&
                          (EndDate == default(DateTime) || date < EndDate)
                          select date).Min().Subtract(SystemTime.Now()).TotalMilliseconds;
            }

            return result;
        }
        public double GetNextWeeklyItemMilliseconds(DayOfWeek today, DateTime compareDate)
        {
            double result = default(double);

            List<TimeSpan> todaysList = _weeklySchedule[today];
            DateTime minimumNextDateTime = default(DateTime);

            foreach (TimeSpan timeSpan in todaysList)
            {
                DateTime current = new DateTime(compareDate.Year, compareDate.Month, compareDate.Day, timeSpan.Hours,
                                                timeSpan.Minutes, timeSpan.Seconds);

                if (current > compareDate)
                {
                    if (minimumNextDateTime == default(DateTime))
                    {
                        minimumNextDateTime = current;
                    }
                    else
                    {
                        if (minimumNextDateTime > current)
                        {
                            minimumNextDateTime = current;
                        }
                    }
                }
            }

            if (minimumNextDateTime != default(DateTime) && minimumNextDateTime > compareDate)
            {
                result = minimumNextDateTime.Subtract(compareDate).TotalMilliseconds;
            }

            return result;
        }
        public double GetNextMonthlyItemMilliseconds(byte month, DateTime compareDate)
        {
            double result = default(double);

            List<TimeSpan> monthList = _monthlySchedule[month];
            DateTime minimumNextDateTime = default(DateTime);

            foreach (TimeSpan timeSpan in monthList)
            {
                DateTime current = new DateTime(compareDate.Year, compareDate.Month, compareDate.Day, timeSpan.Hours,
                                                timeSpan.Minutes, timeSpan.Seconds);

                if (current > compareDate)
                {
                    if (minimumNextDateTime == default(DateTime))
                    {
                        minimumNextDateTime = current;
                    }
                    else
                    {
                        if (minimumNextDateTime > current)
                        {
                            minimumNextDateTime = current;
                        }
                    }
                }
            }

            if (minimumNextDateTime != default(DateTime) && minimumNextDateTime > compareDate)
            {
                result = minimumNextDateTime.Subtract(compareDate).TotalMilliseconds;
            }

            return result;

        }

        public double GetNextItemMilliseconds()
        {
            double result;

            double nextOneTimeItemInMilliseconds = GetNextOneTimeItemInMilliseconds();
            double nextWeeklyItemMilliseconds = GetNextWeeklyItemMilliseconds(SystemTime.Now().DayOfWeek, SystemTime.Now());
            double nextMonthlyItemMilliseconds = GetNextMonthlyItemMilliseconds((byte)SystemTime.Now().Month, SystemTime.Now());

            result = nextOneTimeItemInMilliseconds > 0 ? nextOneTimeItemInMilliseconds : default(double);
            result = nextWeeklyItemMilliseconds > 0
                         ? result > 0
                               ? Math.Min(result, nextWeeklyItemMilliseconds)
                               : nextWeeklyItemMilliseconds
                         : result;

            result = nextMonthlyItemMilliseconds > 0
                         ? result > 0
                               ? Math.Min(result, nextMonthlyItemMilliseconds)
                               : nextMonthlyItemMilliseconds
                         : result;


            return result;
        }

    }
}