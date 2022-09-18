using System.Windows;
using ClientWPF.MVVM.View;
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
            
            Container.Register<MainWindow>(Reuse.Singleton);
            Container.Register<HelloWindow>(Reuse.Singleton);
            Container.Register<MainWindowViewModel>(Reuse.Singleton);
            
            var reg = Container.Resolve<HelloWindow>();
            reg.DataContext = new HelloWindowViewModel(reg);
            reg.Show();
        }
    }
}