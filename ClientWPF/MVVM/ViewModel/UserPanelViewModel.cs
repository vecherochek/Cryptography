using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Input;
using ClientWPF.Core;
using ClientWPF.MVVM.Commands;
using DryIoc;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using LUC;

namespace ClientWPF.MVVM.ViewModel;

public class UserPanelViewModel : ObservableObject
{
    private string _dealkey;
    private string _privateLucKey;
    private string _nLucKey;
    private string _username;
    public ICommand GetDEALkeyCommand { get; set; }
    private MainWindowViewModel _mainwindowVM;

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

    public UserPanelViewModel()
    {
        _mainwindowVM = App.Container.Resolve<MainWindowViewModel>();
        GetDEALkeyCommand = new RelayCommand(GetDEALkey);
    }

    private async void GetDEALkey(object o)
    {
        if (_mainwindowVM.ConnectionCode == 1)
        {
            if (PrivateLUCkey == null || NLUCkey == null)
            {
                _mainwindowVM.Messages = "[memo] Enter LUC keys to get DEAL key\n";
                return;
            }

            _username = _mainwindowVM.UserName;
            var a = await _mainwindowVM.Client.GetKey(_username);
            var privatekey = new LucKey(BigInteger.Parse(PrivateLUCkey),
                BigInteger.Parse(NLUCkey));
            var decrypted = new LUC.LUC().Decrypt(new BigInteger(a.Key.ToArray()), privatekey);
            DEALkey = decrypted.ToString();
            _mainwindowVM.Messages = "[memo] Now you can chatting\n";
            Task.Run(() => Chatting(_mainwindowVM));
            
        }

        else _mainwindowVM.Messages = "[memo] Join the server to get started\n";
    }

    private async void Chatting(MainWindowViewModel vm)
    {
        var dataStream = vm.Client._streamingClient.ChatMessagesStreaming(new Empty());
        await foreach (var messageData in dataStream.ResponseStream.ReadAllAsync())
        {
            //vm.Messages.Add(new MessageBoxControl($"[{DateTime.Now}]{messageData.User}: {messageData.Message}"));
            vm.Messages = $"[{messageData.Time}] {messageData.User} : {messageData.Message.ToStringUtf8()}\n";
        }
    }
}