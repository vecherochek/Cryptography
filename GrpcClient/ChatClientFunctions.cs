using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;

namespace GrpcClient;

public class ChatClientFunctions
{
    private GrpcChannel _channel;
    private Chat.ChatClient _client;
    public ChatMessagesStreaming.ChatMessagesStreamingClient _streamingClient;

    public ChatClientFunctions()
    {
        _channel = GrpcChannel.ForAddress("http://localhost:5259");
        _client = new Chat.ChatClient(_channel);
        _streamingClient = new ChatMessagesStreaming.ChatMessagesStreamingClient(_channel);
    }

    public async Task<ServerResponse> Login(string username)
    {
        return await _client.LoginAsync(new UserRequest {User = username});
    }

    public async Task<Empty> Logout(string username)
    {
        return await _client.LogoutAsync(new UserRequest {User = username});
    }

    public async Task<KeyOutput> GetKey(string username)
    {
        return await _client.GetKeyAsync(new UserRequest {User = username});
    }

    public async Task<Empty> SendKey(byte[] key)
    {
        return await _client.SendKeyAsync(new KeyInput {Key = ByteString.CopyFrom(key)});
    }

    public async Task<Empty> SendMessage(string username, byte[] message, string time)
    {
        return await _client.SendMessageAsync(new MessageInput
        {
            User = username,
            Time = time,
            Message = ByteString.CopyFrom(message)
        });
    }
}