using System;
using System.Linq;
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

namespace ClientWPF.MVVM.ViewModel;

public class UserPanelViewModel : ObservableObject
{
    private string _dealkey;
    private string _privateLucKey;
    private string _nLucKey;
    private string _username;
    public byte[] dealIV;
    public byte[] desIV;
    public bool keysend = false;
    public ICommand GetDEALkeyCommand { get; set; }
    private MainWindowViewModel _mainwindowVM;
    private AsyncServerStreamingCall<ReceivedMessage> dataStream;


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
                _mainwindowVM.Memo("Enter LUC keys to get DEAL key.");
                return;
            }

            _username = _mainwindowVM.UserName;
            try
            {
                var a = await _mainwindowVM.Client.GetKey(_username);
                keysend = true;
                var privatekey = new LucKey(BigInteger.Parse(PrivateLUCkey),
                    BigInteger.Parse(NLUCkey));

                var LucEncoder = new LUC.LUC();
                var decryptedKey = LucEncoder.Decrypt(new BigInteger(a.Key.ToArray()), privatekey);
                var decryptedDesIV = LucEncoder.Decrypt(new BigInteger(a.DesIV.ToArray()), privatekey);
                var decryptedDealIV = LucEncoder.Decrypt(new BigInteger(a.DealIV.ToArray()), privatekey);

                DEALkey = decryptedKey.ToString();
                dealIV = decryptedDealIV.ToByteArray();
                desIV = decryptedDesIV.ToByteArray();
                _mainwindowVM.Memo("Now you can chatting.");
                if (dataStream == null) Task.Run(() => Chatting());
            }
            catch (Exception e)
            {
                _mainwindowVM.Memo("There is no connection to the server.");
                return;
            }
        }

        else _mainwindowVM.Memo("Join the server to get started.");
    }

    private async void Chatting()
    {
        dataStream = _mainwindowVM.Client._streamingClient.ChatMessagesStreaming(new Empty());
        try
        {
            await foreach (var messageData in dataStream.ResponseStream.ReadAllAsync())
            {
                if (_mainwindowVM.Encoder == null)
                {
                    _mainwindowVM.InitCipher(desIV, dealIV);
                }

                var tmp = messageData.Message.ToByteArray();
                var mes = _mainwindowVM.Cipher.Decrypt(tmp, _mainwindowVM.RoundKeys);
                if (messageData.Filename != string.Empty)
                {
                    _mainwindowVM.InvokeInUiThread(new Action(() => _mainwindowVM.MessageControls.Add(
                        new MessageBoxControl(messageData.User, messageData.Time, mes, messageData.Filename))));
                }
                else
                {
                    _mainwindowVM.InvokeInUiThread(new Action(() => _mainwindowVM.MessageControls.Add(
                        new MessageBoxControl(messageData.User, messageData.Time, Encoding.UTF8.GetString(mes)))));
                }
            }
        }
        catch (Exception e)
        {
            _mainwindowVM.Memo("There is no connection to the server.");
            return;
        }
    }
}