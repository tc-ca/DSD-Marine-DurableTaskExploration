using System.Threading.Tasks;

namespace Common.Messaging
{
    public interface IMessageSender
    {
        Task SendAsync(object message, string queueOrTopicName);
    }
}
