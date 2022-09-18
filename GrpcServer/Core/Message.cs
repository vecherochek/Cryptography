namespace GrpcServer.Core;

public class Message
{
    public int MessageId { get; set; }
    public User User { get; set; }
    public byte[] UserMessage { get; set; }
    public string Time { get; set; }
}