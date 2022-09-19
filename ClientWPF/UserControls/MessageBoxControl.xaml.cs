using System.Windows.Controls;
using GrpcClient;

namespace ClientWPF.UserControls;

public partial class MessageBoxControl : UserControl
{
    public string Message { get; set; }
    public string UserName { get; set; }
    public string Time { get; set; }

    public MessageBoxControl(string username, string time, string message)
    {
        InitializeComponent();
        Message = message;
        UserName = username;
        Time = time;
    }
    
}