using System.Windows;
using System.Windows.Input;
using ClientWPF.Core;
using ClientWPF.MVVM.Commands;
using DryIoc;

namespace ClientWPF.MVVM.ViewModel;

public class HelloWindowViewModel : ObservableObject
{
    public ICommand OpenAdminWindowCommand { get; set; }
    public ICommand OpenUserWindowCommand { get; set; }
    private readonly Window _owner;

    public HelloWindowViewModel(Window owner)
    {
        _owner = owner;

        OpenAdminWindowCommand = new RelayCommand(o =>
        {
            var a = App.Container.Resolve<MainWindow>();
            a.DataContext = new MainWindowViewModel(new AdminPanelViewModel());
            a.Show();
            _owner.Close();
        });
        OpenUserWindowCommand = new RelayCommand(o =>
        {
            var a = App.Container.Resolve<MainWindow>();
            a.DataContext = new MainWindowViewModel(new UserPanelViewModel());
            a.Show();
            _owner.Close();
        });
    }
}