using System;

namespace GenericWindowsService.BL
{
    /// <summary>
    /// System Time Class for test purposes.
    /// You can set desired datetime value in test project for SystemTime 
    /// Check for more examples in the test project
    /// </summary>
    /// <example>
    /// SystemTime.Now = () => new DateTime(2010, 1, 1, 18, 0, 0);
    /// </example>
    public static class SystemTime
    {
        private static Func<DateTime> __currentDate = () => DateTime.Now;

        public static Func<DateTime> Now
        {
            get { return __currentDate; }
            internal set { __currentDate = value; }
        }

        // ayende's solution: public static Func<DateTime> Now = () => DateTime.Now;
    }
}
