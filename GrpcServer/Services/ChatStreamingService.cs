using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcServer.Core;

namespace GrpcServer.Services;

public class ChatStreamingService: ChatMessagesStreaming.ChatMessagesStreamingBase
{
    public override async Task ChatMessagesStreaming(Empty request, IServerStreamWriter<ReceivedMessage> responseStream, ServerCallContext context)
    {
        int lastMessageId = -1;
        try
        {
            while (true)
            {
                await Task.Delay(100);
                int lastPosition = 0;

                for (int index = 0; index < MessageQueue.messages.Count(); index++)
                {
                    if (MessageQueue.messages[index].MessageId == lastMessageId)
                    {
                        lastPosition = index++;
                        break;
                    }
                }
                for (int index = lastPosition; index < MessageQueue.messages.Count; index++)
                {
                    if (MessageQueue.messages[index].MessageId != lastMessageId)
                    {
                        var receivedMessage = new ReceivedMessage
                        {
                            User = MessageQueue.messages[index].User.UserName,
                            Time = MessageQueue.messages[index].Time,
                            Message = ByteString.CopyFrom(MessageQueue.messages[index].UserMessage)
                            
                        };
                        lastMessageId = MessageQueue.messages[index].MessageId;
                        await responseStream.WriteAsync(receivedMessage);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("\n" + e.Message + "\n");
        }
    }
}