namespace GenericWindowsService.BL
{
    public interface IGenericServiceItem
    {
        IExecutionSchedule ExecutionSchedule { get; set; }
        IExecutableDefinition ExecutableDefinition { get; set; }

        void InitTimerInterval();
        void StopTimer();

        void Execute();
    }
}