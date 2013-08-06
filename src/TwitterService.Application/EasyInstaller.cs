namespace TwitterService.Application
{
    using System.Configuration.Install;
    using System.ServiceProcess;
    using System.ComponentModel;

    [RunInstaller(true)]
    public partial class EasyInstaller : Installer
    {
        private readonly ServiceProcessInstaller _serviceProcess;
        private readonly ServiceInstaller _serviceInstaller;

        public EasyInstaller()
        {
            InitializeComponent();

            _serviceProcess = new ServiceProcessInstaller { Account = ServiceAccount.NetworkService };
            _serviceInstaller = new ServiceInstaller
            {
                ServiceName = "ColladeTwitterService",
                DisplayName = "Collade Twitter Service",
                Description = "Searching tweets by keywords and saving them to db.",
                StartType = ServiceStartMode.Automatic
            };
            Installers.Add(_serviceProcess);
            Installers.Add(_serviceInstaller);
        }
    }
}
