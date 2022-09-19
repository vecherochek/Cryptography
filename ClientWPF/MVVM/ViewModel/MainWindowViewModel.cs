using System;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using ClientWPF.Core;
using ClientWPF.MVVM.Commands;
using ClientWPF.UserControls;
using GrpcClient;
using static Cryptography.Extensions.ByteArrayExtensions;

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

    /*private string _messages;*/
    public uint ConnectionCode;
    public dynamic CurrentView { get; set; }

    public ObservableCollection<MessageBoxControl> MessageControls { get; set; } =
        new ObservableCollection<MessageBoxControl>();

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

    /*public string Messages
    {
        get => _messages;
        set
        {
            _messages += value;
            OnPropertyChanged();
        }
    }*/

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
        if (UserName != string.Empty)
        {
            try
            {
                var a = await Client.Login(UserName);
                ConnectionCode = a.Code;
                string memo = ConnectionCode == 1
                    ? "You are now connected! Get a DEAL key to start communicating."
                    : a.Message;
                Memo(memo);
            }
            catch (Exception e)
            {
                Memo("Unable to connect to the server.");
                return;
            }
        }
        else
        {
            Memo("Enter the user name.");
        }
    }

    private async void SendMessage(object o)
    {
        if (ConnectionCode == 1)
        {
            if (CurrentView.DEALkey == null)
            {
                Memo("Generate/get DEAL key to start chatting.");
                return;
            }

            if (!CurrentView.keysend)
            {
                Memo("Send/get DEAL key to start chatting.");
                return;
            }

            if (Encoder == null)
            {
                InitCipher(CurrentView.desIV, CurrentView.dealIV);
            }

            if (UserMessage != String.Empty)
            {
                try
                {
                    var mes = Encoding.UTF8.GetBytes(UserMessage);
                    var encrypted = Cipher.Encrypt(mes, RoundKeys);
                    await Client.SendMessage(UserName, encrypted, DateTime.Now.ToString("HH:mm"));
                    UserMessage = String.Empty;
                }
                catch (Exception e)
                {
                    Memo("There is no connection to the server.");
                    return;
                }
            }
        }

        else Memo("Join the server to get started.");
    }

    private async void Close(object o)
    {
        if (ConnectionCode == 1)
            try
            {
                await Client.Logout(UserName);
            }
            catch (Exception e)
            {
                Memo("There is no connection to the server.");
                return;
            }

        Application.Current.Shutdown();
    }

    public void InitCipher(byte[] desIV, byte[] dealIV)
    {
        var key = BigInteger.Parse(CurrentView.DEALkey).ToByteArray();
        Encoder = new DEAL.DEAL(key, desIV);
        Cipher = new CipherContext.CipherContext(Encoder, key, dealIV, "kek")
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

    public void Memo(string massage)
    {
        InvokeInUiThread(new Action(() => MessageControls.Add(
            new MessageBoxControl("[memo]", DateTime.Now.ToString("HH:mm"), massage))));
    }

    /// <summary>
    /// Invoke <see cref="Action"/> in default UI thread.
    /// </summary>
    /// <param name="action"></param>
    public void InvokeInUiThread(Action action)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                return;
            }
        }, DispatcherPriority.Normal);
    }
}