using System.Windows;
using ClientWPF.MVVM.ViewModel;
using DryIoc;

namespace ClientWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Container Container { get; set; }
        
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Container = new Container();

            // Grpc client
            //Views
            Container.Register<MainWindow>(Reuse.Singleton);

            //ViewModels

            var reg = Container.Resolve<MainWindow>();
            //reg.DataContext = Container.Resolve<MainWindowViewModel>();
            reg.Show();
        }
    }
}