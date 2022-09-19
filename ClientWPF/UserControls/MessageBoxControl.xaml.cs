using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using ClientWPF.MVVM.Commands;
using Microsoft.Win32;

namespace ClientWPF.UserControls;

public partial class MessageBoxControl : UserControl
{
    public string Message { get; set; }
    public string UserName { get; set; }
    public string Time { get; set; }
    public byte[] UserFile { get; set; }
    public string ColorBrush { get; set; }
    public ICommand DownloadFileCommand { get; set; }
    public string ButtonVisibility { get; set; }

    public MessageBoxControl(string username, string time, string message)
    {
        InitializeComponent();
        Message = message;
        UserName = username;
        Time = time;
        ColorBrush = "White";
        ButtonVisibility = "Collapsed";
    }

    public MessageBoxControl(string username, string time, byte[] file, string filename)
    {
        InitializeComponent();
        Message = filename;
        UserName = username;
        Time = time;
        UserFile = file;
        ColorBrush = "LightGray";
        ButtonVisibility = "Visible";
        DownloadFileCommand = new RelayCommand(DownloadFile);
    }

    private async void DownloadFile(object o)
    {
        var saveFileDialog = new SaveFileDialog();
        saveFileDialog.FileName = Message;
        if (saveFileDialog.ShowDialog() == true)
        {
            await File.WriteAllBytesAsync(saveFileDialog.FileName, UserFile);
        }
    }
}