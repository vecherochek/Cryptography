using System.Windows.Input;
using ClientWPF.Core;
using ClientWPF.MVVM.Commands;

namespace ClientWPF.MVVM.ViewModel;

public class UserPanelViewModel : ObservableObject
{
    private string _dealkey;
    private string _privateLucKey;
    private string _nLucKey;
    public ICommand GetDEALkeyCommand { get; set; }
    public UserPanelViewModel()
    {
        GetDEALkeyCommand = new RelayCommand(o =>
        {
            
        });
    }

    public string DEALkey
    {
        get => _dealkey;
        set
        {
            _dealkey = value;
            OnPropertyChanged();
        }
    }
    public string PrivateLUCkey
    {
        get => _privateLucKey;
        set
        {
            _privateLucKey = value;
            OnPropertyChanged();
        }
    }

    public string NLUCkey
    {
        get => _nLucKey;
        set
        {
            _nLucKey = value;
            OnPropertyChanged();
        }
    }
}