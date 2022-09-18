﻿using System;
using System.Numerics;
using System.Windows.Input;
using ClientWPF.Core;
using ClientWPF.MVVM.Commands;
using GrpcClient;

namespace ClientWPF.MVVM.ViewModel;

public class MainWindowViewModel : ObservableObject
{
    private string _answer;
    private string _userName;
    public ChatClientFunctions Client { get; set; }
    public ICommand ConnectToTheServerCommand { get; set; }

    public object CurrentView { get; set; }

    public string UserName
    {
        get => _userName;
        set
        {
            _userName = value;
            OnPropertyChanged();
        }
    }

    public string Answer
    {
        get => _answer;
        set
        {
            _answer = value;
            OnPropertyChanged();
        }
    }

    public MainWindowViewModel()
    {
        ConnectToTheServerCommand = new RelayCommand(ConnectToTheServer);
    }

    private async void ConnectToTheServer(object o)
    {
        Client = new ChatClientFunctions();
        var a = await Client.Login(UserName);
        Answer = a.Code == 0 ? "You are now connected!" : a.Message;
    }
}