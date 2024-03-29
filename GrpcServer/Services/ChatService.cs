﻿using System.Numerics;
using System.Text;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using GrpcServer.Core;
using GrpcServer;

namespace GrpcServer.Services;

using Grpc.Core;

public class ChatService : Chat.ChatBase
{
    private readonly ILogger<ChatService> _logger;
    private int _usersId = 1;
    private static ByteString ServerKey { get; set; }
    private static ByteString ServerDesIV { get; set; }
    private static ByteString ServerDealIV { get; set; }

    /*private readonly User _server = new User
    {
        UserName = "SERVER"
    };*/

    public ChatService(ILogger<ChatService> logger)
    {
        _logger = logger;
    }

    public override Task<ServerResponse> Login(UserRequest request, ServerCallContext context)
    {
        try
        {
            UserDictionary.users.Add(
                request.User,
                _usersId);
        }
        catch (Exception e)
        {
            return Task.FromResult(new ServerResponse
            {
                Code = 0,
                Message = "Such a username already exists"
            });
        }

        _usersId++;
        _logger.LogInformation($"\n[{DateTime.UtcNow}]{request.User} has connected!\n");
        /*MessageQueue.messages.Add(new Message
            {
                MessageId = new Random().Next(0, int.MaxValue),
                User = _server,
                UserMessage = Encoding.Default.GetBytes($"\n[{DateTime.Now:HH:mm}]{request.User} has connected!\n"),
                Time = DateTime.Now.ToString("HH:mm")
            }
        );*/
        return Task.FromResult(new ServerResponse
        {
            Code = 1
        });
    }

    public override Task<Empty> Logout(UserRequest request, ServerCallContext context)
    {
        _logger.LogInformation($"\n[{DateTime.UtcNow}] {request.User} has disconnected!\n");
        UserDictionary.users.Remove(request.User);
        /*MessageQueue.messages.Add(new Message
            {
                MessageId = new Random().Next(0, int.MaxValue),
                User = _server,
                Time = DateTime.Now.ToString("HH:mm"),
                UserMessage = Encoding.Default.GetBytes($"\n[{DateTime.Now:HH:mm}] {request.User} has disconnected!\n")
            }
        );*/
        if (UserDictionary.users.Count == 0) MessageQueue.messages.Clear();
        
        return Task.FromResult(new Empty());
    }

    public override Task<Empty> SendMessage(MessageInput request, ServerCallContext context)
    {
        _logger.LogInformation(request.Filename == string.Empty
            ? $"\n[{request.Time}]{request.User}: {request.Message.ToStringUtf8()}\n"
            : $"\n[{request.Time}]{request.User}: file {request.Filename}\n");
        MessageQueue.messages.Add(new Message
            {
                MessageId = new Random().Next(0, int.MaxValue),
                User = new User
                {
                    UserName = request.User
                },
                Time = request.Time,
                UserMessage = request.Message.ToByteArray(),
                FileName = request.Filename
            }
        );
        return Task.FromResult(new Empty());
    }

    public override Task<Empty> SendKey(KeyInput request, ServerCallContext context)
    {
        _logger.LogInformation($"\n[{DateTime.UtcNow}]: The symmetric key was received by the server\n");
        ServerKey = request.Key;
        ServerDesIV = request.DesIV;
        ServerDealIV = request.DealIV;
        MessageQueue.messages.Clear();
        return Task.FromResult(new Empty());
    }

    public override Task<KeyOutput> GetKey(UserRequest request, ServerCallContext context)
    {
        _logger.LogInformation($"\n[{DateTime.UtcNow}]: {request.User} received the key\n");
        return Task.FromResult(new KeyOutput
        {
            Key = ServerKey,
            DealIV = ServerDealIV,
            DesIV = ServerDesIV
        });
    }
}