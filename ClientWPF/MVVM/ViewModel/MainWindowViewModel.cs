using System;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Input;
using ClientWPF.Core;
using ClientWPF.MVVM.Commands;
using GrpcClient;

namespace ClientWPF.MVVM.ViewModel;

public class MainWindowViewModel : ObservableObject
{
    private string _userName;
    private string _userMessage;
    public ChatClientFunctions Client { get; set; }
    public ICommand ConnectToTheServerCommand { get; set; }
    public ICommand CloseCommand { get; set; }
    public ICommand SendMessageCommand { get; set; }
    public DEAL.DEAL Encoder;
    public CipherContext.CipherContext Cipher;
    public byte[][] RoundKeys;

    private string _messages;
    public uint ConnectionCode;
    public dynamic CurrentView { get; set; }

    /*public ObservableCollection<MessageBoxControl> Messages { get; set; } =
        new ObservableCollection<MessageBoxControl>();*/

    public string UserName
    {
        get => _userName;
        set
        {
            _userName = value;
            OnPropertyChanged();
        }
    }

    public string UserMessage
    {
        get => _userMessage;
        set
        {
            _userMessage = value;
            OnPropertyChanged();
        }
    }

    public string Messages
    {
        get => _messages;
        set
        {
            _messages += value;
            OnPropertyChanged();
        }
    }

    public MainWindowViewModel()
    {
        ConnectToTheServerCommand = new RelayCommand(ConnectToTheServer);
        CloseCommand = new RelayCommand(Close);
        SendMessageCommand = new RelayCommand(SendMessage);
        ConnectionCode = 0;
    }

    private async void ConnectToTheServer(object o)
    {
        Client = new ChatClientFunctions();
        var a = await Client.Login(UserName);
        ConnectionCode = a.Code;
        Messages = ConnectionCode == 1 ? "[memo] You are now connected! Get a DEAL key to start communicating.\n" : "[memo] " + a.Message;
    }

    private async void SendMessage(object o)
    {
        if (ConnectionCode == 1)
        {
            if (CurrentView.DEALkey == null)
            {
                Messages = "[memo] Generate/get DEAL key to start chatting\n";
                return;
            }

            if (Encoder == null)
            {
                InitCipher();
            }

            /*var mes =  Encoding.UTF8.GetBytes(UserMessage);
            var encrypted = Cipher.Encrypt(mes, RoundKeys);
            await Client.SendMessage(UserName, encrypted, DateTime.Now.ToString("HH:mm"));*/
            await Client.SendMessage(UserName, Encoding.UTF8.GetBytes(UserMessage), DateTime.Now.ToString("HH:mm"));
            UserMessage = String.Empty;
        }

        else Messages = "[memo] Join the server to get started\n";
    }

    private async void Close(object o)
    {
        if (ConnectionCode == 1) await Client.Logout(UserName);
        Application.Current.Shutdown();
    }

    public void InitCipher()
    {
        var key = BigInteger.Parse(CurrentView.DEALkey).ToByteArray();
        Encoder = new DEAL.DEAL(key);
        Cipher = new CipherContext.CipherContext(Encoder, key, Encoder.GenerateIV(), "kek")
        {
            //EncryptionMode = EncryptionModeList.OFB
            //EncryptionMode = EncryptionModeList.CFB
            //EncryptionMode = EncryptionModeList.CBC
    
            //EncryptionMode = EncryptionModeList.ECB
            //EncryptionMode = EncryptionModeList.CTR
            //EncryptionMode = EncryptionModeList.RD
            EncryptionMode = CipherContext.CipherContext.EncryptionModeList.RDH
        };
        RoundKeys = Cipher.GenerateRoundKeys();
    }
}