using Messages.Commands;
using NServiceBus;

namespace Commons.Initializers
{
    public class TransportInitializer : INeedInitialization
    {
        public void Customize(EndpointConfiguration endpointConfiguration)
        {
            endpointConfiguration.EnableInstallers();
            var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
            transport.ConnectionString("host=localhost;username=guest;password=guest");
            transport.UseConventionalRoutingTopology();
            endpointConfiguration.AuditProcessedMessagesTo("personalQueueForAudit");

            // Command Routing
            var routing = transport.Routing();
            routing.RouteToEndpoint(typeof(PlaceOrder), "Sales");
            routing.RouteToEndpoint(typeof(CancelOrder), "Sales");
        }
    }

    public class SerializationInitializer : INeedInitialization
    {
        public void Customize(EndpointConfiguration endpointConfiguration)
        {
            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
        }
    }

}
