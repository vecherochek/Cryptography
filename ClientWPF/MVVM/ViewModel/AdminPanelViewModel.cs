using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ClientWPF.Core;
using ClientWPF.MVVM.Commands;
using DryIoc;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using LUC;
using static Cryptography.Extensions.ByteArrayExtensions;

namespace ClientWPF.MVVM.ViewModel;

public class AdminPanelViewModel : ObservableObject
{
    private string _dealkey;
    private string _publicLucKey;
    private string _privateLucKey;
    private string _nLucKey;
    private byte[] _dealkeybyte;
    private MainWindowViewModel _mainwindowVM;
    public ICommand GenerateDEALkeyCommand { get; set; }
    public ICommand GenerateLUCkeysCommand { get; set; }

    public ICommand SendDEALkeyCommand { get; set; }

    public KeyGenerator LUCkeys { get; set; }

    public AdminPanelViewModel()
    {
        _mainwindowVM = App.Container.Resolve<MainWindowViewModel>();
        GenerateDEALkeyCommand = new RelayCommand(o =>
        {
            _dealkeybyte = GenerateRandomByteArray(16);
            DEALkey = new BigInteger(_dealkeybyte).ToString();
        });
        GenerateLUCkeysCommand = new RelayCommand(GenerateLUCkeys);
        SendDEALkeyCommand = new RelayCommand(SendDEALkey);
    }

    private async void SendDEALkey(object o)
    {
        if (DEALkey == null)
        {
            _mainwindowVM.Messages = "[memo] Generate DEAL key to send it\n";
            return;
        }

        if (PrivateLUCkey == null || PublicLUCkey == null || NLUCkey == null)
        {
            _mainwindowVM.Messages = "[memo] Generate LUC keys to send DEAL key\n";
            return;
        }

        if (_mainwindowVM.ConnectionCode == 1)
        {
            var encryptedDealKey = new LUC.LUC().Encrypt(new BigInteger(_dealkeybyte), LUCkeys.PublicKey);
            await _mainwindowVM.Client.SendKey(encryptedDealKey.ToByteArray());
            _mainwindowVM.Messages = "[memo] Now you can chatting\n";
            Task.Run(() => Chatting());
        }

        else _mainwindowVM.Messages = "[memo] Join the server to get started\n";
    }

    private void GenerateLUCkeys(object o)
    {
        if (DEALkey == null)
        {
            _mainwindowVM.Messages = "[memo] Generate DEAL key to generate LUC keys\n";
            return;
        }

        LUCkeys = new KeyGenerator(new BigInteger(Encoding.Default.GetBytes(DEALkey)));
        PublicLUCkey = LUCkeys.PublicKey.Key.ToString();
        PrivateLUCkey = LUCkeys.PrivateKey.Key.ToString();
        NLUCkey = LUCkeys.PrivateKey.N.ToString();
    }

    private async void Chatting()
    {
        var dataStream = _mainwindowVM.Client._streamingClient.ChatMessagesStreaming(new Empty());
        await foreach (var messageData in dataStream.ResponseStream.ReadAllAsync())
        {
            //vm.Messages.Add(new MessageBoxControl($"[{DateTime.Now}]{messageData.User}: {messageData.Message}"));
            if (_mainwindowVM.Encoder == null)
            {
                _mainwindowVM.InitCipher();
            }
            /*var tmp = Encoding.UTF8.GetBytes(messageData.Message.ToStringUtf8());
            var mes = _mainwindowVM.Cipher.Decrypt(tmp, _mainwindowVM.RoundKeys);
            _mainwindowVM.Messages =
                $"[{messageData.Time}] {messageData.User} : {Encoding.UTF8.GetString(mes)}\n";*/
            _mainwindowVM.Messages =
                $"[{messageData.Time}] {messageData.User} : {messageData.Message.ToStringUtf8()}\n";
        }
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

    public string PublicLUCkey
    {
        get => _publicLucKey;
        set
        {
            _publicLucKey = value;
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