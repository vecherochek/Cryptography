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
    private static ByteString ServerKey{ get; set; }

    private readonly User _server = new User
    {
        UserName = "SERVER"
    };

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
                Code = 1,
                Message = "Such a username already exists"
            });
        }

        _usersId++;
        var message = $"\n[{DateTime.UtcNow}]{request.User} has connected!\n";
        _logger.LogInformation(message);
        MessageQueue.messages.Add(new Message
            {
                MessageId = new Random().Next(0, int.MaxValue),
                User = _server,
                UserMessage = Encoding.Default.GetBytes(message)
            }
        );
        return Task.FromResult(new ServerResponse
        {
            Code = 0
        });
    }

    public override Task<Empty> Logout(UserRequest request, ServerCallContext context)
    {
        var message = $"\n[{DateTime.UtcNow}]{request.User} has disconnected!\n";
        _logger.LogInformation(message);
        UserDictionary.users.Remove(request.User);
        MessageQueue.messages.Add(new Message
            {
                MessageId = new Random().Next(0, int.MaxValue),
                User = _server,
                UserMessage = Encoding.Default.GetBytes(message)
            }
        );
        return Task.FromResult(new Empty());
    }

    public override Task<Empty> SendMessage(MessageInput request, ServerCallContext context)
    {
        _logger.LogInformation($"\n[{DateTime.UtcNow}]{request.User}: {request.Message}\n");
        MessageQueue.messages.Add(new Message
            {
                MessageId = new Random().Next(0, int.MaxValue),
                User = new User
                {
                    UserName = request.User
                },
                UserMessage = request.Message.ToByteArray()
            }
        );
        return Task.FromResult(new Empty());
    }

    public override Task<Empty> SendKey(KeyInput request, ServerCallContext context)
    {
        _logger.LogInformation($"\n[{DateTime.UtcNow}]: A new symmetric algorithm key has been created\n");
        ServerKey = request.Key;
        
        return Task.FromResult(new Empty());
    }

    public override Task<KeyOutput> GetKey(UserRequest request, ServerCallContext context)
    {
        _logger.LogInformation($"\n[{DateTime.UtcNow}]: {request.User} received the key\n");
        return Task.FromResult(
            new KeyOutput
            {
                Key = ServerKey
            });
    }
}