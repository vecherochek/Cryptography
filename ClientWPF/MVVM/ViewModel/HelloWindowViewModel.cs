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
        OpenAdminWindowCommand = new RelayCommand(o => OpenMainWindow(new AdminPanelViewModel()));
        OpenUserWindowCommand = new RelayCommand(o => OpenMainWindow(new UserPanelViewModel()));
    }

    private void OpenMainWindow(object panel)
    {
        var a = App.Container.Resolve<MainWindow>();
        var viewModel = App.Container.Resolve<MainWindowViewModel>();
        viewModel.CurrentView = panel;
        a.DataContext = viewModel;
        a.Show();
        _owner.Close();
    }
}