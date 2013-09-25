using System.ServiceProcess;
using GenericWindowsService.BL;

namespace GenericWindowsService.Service
{
    public partial class Service1 : ServiceBase
    {
        private readonly IServiceManager _serviceManager;
        
        public Service1()
        {
            InitializeComponent();
            _serviceManager = new ServiceManager();
        }

        protected override void OnStart(string[] args)
        {
            _serviceManager.Start();
        }

        protected override void OnStop()
        {
            _serviceManager.Stop();
        }
    }
}
