using Microsoft.Extensions.Configuration;

namespace BLL.Eventhub
{
    public class OrderEventSenderFactory : IOrderEventSenderFactory
    {
        public IOrderEventSender Create(IConfiguration config, string eventHubName)
        {
            return new OrderEventSender(config, eventHubName); ;
        }
    }
}

