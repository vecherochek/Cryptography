using System.Linq;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Input;
using ClientWPF.Core;
using ClientWPF.MVVM.Commands;
using DryIoc;
using GrpcClient;
using LUC;

namespace ClientWPF.MVVM.ViewModel;

public class UserPanelViewModel : ObservableObject
{
    private string _dealkey;
    private string _privateLucKey;
    private string _nLucKey;
    private string _username;

    public ICommand GetDEALkeyCommand { get; set; }
    public UserPanelViewModel()
    {
        GetDEALkeyCommand = new RelayCommand(GetDEALkey);
    }

    private async void GetDEALkey(object o)
    {
        var mainwindowVM = App.Container.Resolve<MainWindowViewModel>();
        _username = mainwindowVM.UserName;
        
        var a = await mainwindowVM.Client.GetKey(_username);
        var privatekey = new LucKey(BigInteger.Parse(PrivateLUCkey),
            BigInteger.Parse(NLUCkey));
        var decrypted = new LUC.LUC().Decrypt(new BigInteger(a.Key.ToArray()), privatekey);
        DEALkey = decrypted.ToString();
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