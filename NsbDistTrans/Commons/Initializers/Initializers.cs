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

    // BAD IDEA!
    //public class SqlServerOutboxInitializer : INeedInitialization
    //{
    //    public void Customize(EndpointConfiguration endpointConfiguration)
    //    {
    //        var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
    //        var connection = "Data Source=localhost;Initial Catalog=NsbDistMongo;User Id=sa;pwd=Docker@123";
    //        persistence.SqlDialect<SqlDialect.MsSqlServer>();
    //        persistence.ConnectionBuilder(
    //            connectionBuilder: () =>
    //            {
    //                return new SqlConnection(connection);
    //            });
    //        var subscriptions = persistence.SubscriptionSettings();
    //        subscriptions.CacheFor(TimeSpan.FromMinutes(1));
    //    }
    //}

    public class DebugRetyPolicy :INeedInitialization
    {
        public void Customize(EndpointConfiguration configuration)
        {
            var recoverability = configuration.Recoverability();
            configuration.LimitMessageProcessingConcurrencyTo(1);
            recoverability.Immediate(
                customizations: immediate => { immediate.NumberOfRetries(0); });
            recoverability.Delayed(
                customizations: delayed => { delayed.NumberOfRetries(2); });

        }
    }

}
