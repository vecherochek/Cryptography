using System.Numerics;
using System.Text;
using System.Windows.Input;
using ClientWPF.Core;
using ClientWPF.MVVM.Commands;
using LUC;
using static Cryptography.Extensions.ByteArrayExtensions;

namespace ClientWPF.MVVM.ViewModel;

public class AdminPanelViewModel : ObservableObject
{
    private string _dealkey;
    private string _publicLucKey;
    private string _privateLucKey;
    private string _nLucKey;
    public ICommand GenerateDEALkeyCommand { get; set; }
    public ICommand GenerateLUCkeysCommand { get; set; }
    public KeyGenerator LUCkeys { get; set; }

    public AdminPanelViewModel()
    {
        GenerateDEALkeyCommand = new RelayCommand(o =>
        {
            DEALkey = new BigInteger(GenerateRandomByteArray(16)).ToString();
        });
        GenerateLUCkeysCommand = new RelayCommand(o =>
        {
            LUCkeys = new KeyGenerator(new BigInteger(Encoding.Default.GetBytes(DEALkey)));
            PublicLUCkey = LUCkeys.PublicKey.Key.ToString();
            PrivateLUCkey = LUCkeys.PrivateKey.Key.ToString();
            NLUCkey = LUCkeys.PrivateKey.N.ToString();
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