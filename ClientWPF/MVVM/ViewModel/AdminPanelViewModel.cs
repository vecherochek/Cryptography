using System;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ClientWPF.Core;
using ClientWPF.MVVM.Commands;
using ClientWPF.UserControls;
using DryIoc;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcClient;
using LUC;
using static Cryptography.Extensions.ByteArrayExtensions;

namespace ClientWPF.MVVM.ViewModel;

public class AdminPanelViewModel : ObservableObject
{
    private string _dealkey;
    private string _publicLucKey;
    private string _privateLucKey;
    private string _nLucKey;
    public byte[] dealIV;
    public byte[] desIV;
    private byte[] _dealkeybyte;
    public bool keysend = false;
    private AsyncServerStreamingCall<ReceivedMessage> dataStream;

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
            keysend = false;
            _dealkeybyte = GenerateRandomByteArray(24);
            DEALkey = new BigInteger(_dealkeybyte).ToString();
            PrivateLUCkey = string.Empty;
            PublicLUCkey = string.Empty;
            NLUCkey = string.Empty;
        });
        GenerateLUCkeysCommand = new RelayCommand(GenerateLUCkeys);
        SendDEALkeyCommand = new RelayCommand(SendDEALkey);
    }

    private async void SendDEALkey(object o)
    {
        if (DEALkey == null)
        {
            _mainwindowVM.Memo("Generate DEAL key to send it.");
            return;
        }

        if (PrivateLUCkey == string.Empty || PublicLUCkey == string.Empty || NLUCkey == string.Empty)
        {
            _mainwindowVM.Memo("Generate LUC keys to send DEAL key.");
            return;
        }

        if (_mainwindowVM.ConnectionCode == 1)
        {
            try
            {
                var LucEncoder = new LUC.LUC();
                dealIV = GenerateRandomByteArray(16);
                desIV = GenerateRandomByteArray(8);
                var encryptedDealKey = LucEncoder.Encrypt(new BigInteger(_dealkeybyte), LUCkeys.PublicKey);
                var encryptedDealIV = LucEncoder.Encrypt(new BigInteger(dealIV), LUCkeys.PublicKey);
                var encryptedDesIV = LucEncoder.Encrypt(new BigInteger(desIV), LUCkeys.PublicKey);
                await _mainwindowVM.Client.SendKey(
                    encryptedDealKey.ToByteArray(),
                    encryptedDealIV.ToByteArray(),
                    encryptedDesIV.ToByteArray());
            }
            catch (Exception e)
            {
                _mainwindowVM.Memo("There is no connection to the server.");
                return;
            }

            keysend = true;
            _mainwindowVM.Memo("Now you can chatting.");
            if (dataStream == null) Task.Run(() => Chatting());
        }

        else _mainwindowVM.Memo("Join the server to get started.");
    }

    private void GenerateLUCkeys(object o)
    {
        if (DEALkey == null)
        {
            _mainwindowVM.Memo("Generate DEAL key to generate LUC keys.");
            return;
        }

        LUCkeys = new KeyGenerator(new BigInteger(Encoding.Default.GetBytes(DEALkey)));
        PublicLUCkey = LUCkeys.PublicKey.Key.ToString();
        PrivateLUCkey = LUCkeys.PrivateKey.Key.ToString();
        NLUCkey = LUCkeys.PrivateKey.N.ToString();
    }

    private async void Chatting()
    {
        dataStream = _mainwindowVM.Client._streamingClient.ChatMessagesStreaming(new Empty());
        try
        {
            await foreach (var messageData in dataStream.ResponseStream.ReadAllAsync())
            {
                //_mainwindowVM.Messages.Add(new MessageBoxControl($"[{DateTime.Now}]{messageData.User}: {messageData.Message}"));
                if (_mainwindowVM.Encoder == null)
                {
                    _mainwindowVM.InitCipher(desIV, dealIV);
                }

                var tmp = messageData.Message.ToByteArray();
                var mes = _mainwindowVM.Cipher.Decrypt(tmp, _mainwindowVM.RoundKeys);
                /*_mainwindowVM.Messages =
                    $"[{messageData.Time}] {messageData.User} : {Encoding.UTF8.GetString(mes)}\n";*/
                _mainwindowVM.InvokeInUiThread(new Action(() => _mainwindowVM.MessageControls.Add(
                    new MessageBoxControl(messageData.User, messageData.Time, Encoding.UTF8.GetString(mes)))));
            }
        }
        catch (Exception e)
        {
            _mainwindowVM.Memo("There is no connection to the server.");
            return;
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