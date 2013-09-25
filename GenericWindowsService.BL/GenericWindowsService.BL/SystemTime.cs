using System;

namespace GenericWindowsService.BL
{
    public static class SystemTime
    {
        private static Func<DateTime> __currentDate = () => DateTime.Now;

        public static Func<DateTime> Now
        {
            get { return __currentDate; }
            internal set { __currentDate = value; }
        }

        // ayende's spublic static Func<DateTime> Now = () => DateTime.Now;
    }
}
