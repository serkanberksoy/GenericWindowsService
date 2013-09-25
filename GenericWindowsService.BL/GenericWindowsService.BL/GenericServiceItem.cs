using System;
using System.Reflection;
using System.Timers;

namespace GenericWindowsService.BL
{
    public class GenericServiceItem : IGenericServiceItem
    {
        public IExecutionSchedule ExecutionSchedule { get; set; }
        public IExecutableDefinition ExecutableDefinition { get; set; }
        private Timer ServiceTimer { get; set; }

        public GenericServiceItem()
        {
            ExecutionSchedule = new ServiceItemExecutionSchedule();
            ExecutableDefinition = new ServiceItemExecutableDefinition();
            ServiceTimer = new Timer();
            ServiceTimer.Elapsed += ServiceTimerElapsed;
            InitTimerInterval();
        }

        public void InitTimerInterval()
        {
            double interval = ExecutionSchedule.GetNextItemMilliseconds();

            if (interval > default(int))
            {
                ServiceTimer.Interval = interval;
                ServiceTimer.Start();
            }
        }

        public void StopTimer()
        {
            ServiceTimer.Stop();
        }

        private void ServiceTimerElapsed(object sender, ElapsedEventArgs e)
        {
            ServiceTimer.Stop();

            try
            {
                Execute();
            }
            catch
            {
            }

            InitTimerInterval();
        }

        public void Execute()
        {
            Assembly assembly = Assembly.LoadFile(ExecutableDefinition.GetFullPath());
            Type type = assembly.GetType(ExecutableDefinition.FullyQualifiedClassName);
            var obj = Activator.CreateInstance(type);

            type.InvokeMember(
                name: ExecutableDefinition.MethodName, 
                invokeAttr: BindingFlags.Default | BindingFlags.InvokeMethod, 
                binder: null, 
                target: obj, 
                args: null);
        }
    }
}