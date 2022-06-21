using Microsoft.Extensions.Configuration;

namespace BLL.Eventhub
{
    public interface IOrderEventSenderFactory
    {
        IOrderEventSender Create(IConfiguration config, string eventHubName);
    }
}
