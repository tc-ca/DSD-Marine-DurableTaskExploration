using System.Text.Json;

namespace Common.Messaging
{
    public static class MessageParser
    {
        public static TMessage Parse<TMessage>(string serializedMessage)
        {
             TMessage message = (TMessage)JsonSerializer.Deserialize(serializedMessage, typeof(TMessage));
            return message;
        }
    }
}
