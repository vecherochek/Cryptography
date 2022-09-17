using ClientWPF.Core;

namespace ClientWPF.MVVM.ViewModel;

public class MainWindowViewModel : ObservableObject
{
    public object CurrentView { get; set; }

    public MainWindowViewModel()
    {
        CurrentView = new AdminPanelViewModel();
    }
}