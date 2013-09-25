namespace GenericWindowsService.BL
{
    public interface IServiceManager
    {
        void InitTimerInterval();
        void SetAllServiceTimers();

        void Start();
        void Stop();
    }
}