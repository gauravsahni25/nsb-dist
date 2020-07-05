using Messages.Commands;
using Messages.Messages;
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
            routing.RouteToEndpoint(typeof(RequestCommand), "MongoActor");
            routing.RouteToEndpoint(typeof(RequestMessage), "MongoActor");
            routing.RouteToEndpoint(typeof(ResponseCommand), "ClientUI");
            // routing.DoNotEnforceBestPractices(); - To demo Request Reply
        }
    }

    public class SerializationInitializer : INeedInitialization
    {
        public void Customize(EndpointConfiguration endpointConfiguration)
        {
            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
        }
    }

    public class DebugSettingsInitializer : INeedInitialization
    {
        public void Customize(EndpointConfiguration endpointConfiguration)
        {
            // debug settings
            endpointConfiguration.LimitMessageProcessingConcurrencyTo(1);
            var recoverability = endpointConfiguration.Recoverability();
            recoverability.Immediate(
                customizations: immediate => { immediate.NumberOfRetries(0); });
            recoverability.Delayed(
                customizations: delayed => { delayed.NumberOfRetries(0); });
        }
    }

}
