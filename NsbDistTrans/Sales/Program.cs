using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NServiceBus;
using NServiceBus.Persistence;
using NServiceBus.Persistence.Sql;
using NServiceBus.Pipeline;
using Sales.SqlStuffForBusiness;

namespace Sales
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string endpointName = "Sales";
            
            Console.Title = endpointName;

            var endpointConfiguration = ConfigureEndpoint(endpointName);
            var endpointInstance = await Endpoint
                                .Start(endpointConfiguration)
                                .ConfigureAwait(false);

            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();

            await endpointInstance
                .Stop()
                .ConfigureAwait(false);
        }

        private static EndpointConfiguration ConfigureEndpoint(string endpointName)
        {
            var endpointConfiguration = new EndpointConfiguration(endpointName);
            ConfigurePersistence(endpointConfiguration);
            return endpointConfiguration;
        }

        private static void ConfigurePersistence(EndpointConfiguration endpointConfiguration)
        {
            var sqlServerPersistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            var connection = "Data Source=localhost;Initial Catalog=NsbDistMongo;User Id=sa;pwd=Docker@123";
            sqlServerPersistence.SqlDialect<SqlDialect.MsSqlServer>();
            sqlServerPersistence.ConnectionBuilder(
                connectionBuilder: () =>
                {
                    return new SqlConnection(connection);
                });
            var subscriptions = sqlServerPersistence.SubscriptionSettings();
            subscriptions.CacheFor(TimeSpan.FromMinutes(1));

            // Create Synchronized EF DBContext
            var pipeline = endpointConfiguration.Pipeline;
            pipeline.Register(new UnitOfWorkSetupBehavior(storageSession =>
            {
                var dbConnection = storageSession.SqlPersistenceSession().Connection;

                var context = new ToDoSqlContext(new DbContextOptionsBuilder<ToDoSqlContext>()
                    .UseSqlServer(dbConnection)
                    .Options);

                //Use the same underlying ADO.NET transaction
                context.Database.UseTransaction(storageSession.SqlPersistenceSession().Transaction);

                //Call SaveChanges before completing storage session
                storageSession
                    .SqlPersistenceSession()
                    .OnSaveChanges(x =>
                    {
                        // ReSharper disable once ConvertToLambdaExpression
                        return context.SaveChangesAsync();
                        // set break point here... and Kill rabbit
                    });

                return context;
            }), "Sets up unit of work for the message");


            // Enable Outbox
            var outboxSettings = endpointConfiguration.EnableOutbox();
            outboxSettings.UsePessimisticConcurrencyControl();
            outboxSettings.KeepDeduplicationDataFor(TimeSpan.FromDays(30));
        }
    }
}

public static class EntityFrameworkUnitOfWorkContextExtensions
{
    public static ToDoSqlContext DataContext(this IMessageHandlerContext context)
    {
        var uow = context.Extensions.Get<EntityFrameworkUnitOfWork>();
        return uow.GetDataContext(context.SynchronizedStorageSession);
    }
}

public class UnitOfWorkSetupBehavior : Behavior<IIncomingLogicalMessageContext>
{
    readonly Func<SynchronizedStorageSession, ToDoSqlContext> _contextFactory;

    public UnitOfWorkSetupBehavior(Func<SynchronizedStorageSession, ToDoSqlContext> contextFactory)
    {
        this._contextFactory = contextFactory;
    }

    public override async Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
    {
        var uow = new EntityFrameworkUnitOfWork(_contextFactory);
        context.Extensions.Set(uow);
        await next().ConfigureAwait(false);
        context.Extensions.Remove<EntityFrameworkUnitOfWork>();
    }
}

public class EntityFrameworkUnitOfWork
{
    readonly Func<SynchronizedStorageSession, ToDoSqlContext> _contextFactory;
    ToDoSqlContext _context;

    public EntityFrameworkUnitOfWork(Func<SynchronizedStorageSession, ToDoSqlContext> contextFactory)
    {
        this._contextFactory = contextFactory;
    }

    public ToDoSqlContext GetDataContext(SynchronizedStorageSession storageSession)
    {
        if (_context == null)
        {
            _context = _contextFactory(storageSession);
        }
        return _context;
    }
}


//endpointConfiguration.RegisterComponents(component =>
//{
//component.ConfigureComponent(builder =>
//{
//    var session = builder.Build<ISqlStorageSession>();

//// Build Options with connection
//    DbContextOptionsBuilder contextOptionsBuilder = new DbContextOptionsBuilder();
//    contextOptionsBuilder.UseSqlServer(session.Connection);

//    // Set options
//    var context = new ToDoSqlContext(contextOptionsBuilder.Options);
//    return context;
//}, 
//DependencyLifecycle.InstancePerUnitOfWork);
//});