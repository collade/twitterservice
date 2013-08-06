namespace TwitterService.Application
{
    using System.ServiceProcess;

    partial class TwitterWindowsService : ServiceBase
    {
        public TwitterWindowsService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Bootstrapper.Initialize();
        }

        protected override void OnStop()
        {
        }
    }
}
