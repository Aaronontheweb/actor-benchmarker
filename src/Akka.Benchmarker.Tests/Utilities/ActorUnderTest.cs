// -----------------------------------------------------------------------
//  <copyright file="ActorUnderTest.cs" company="Akka.NET Project">
//      Copyright (C) 2015-2024 Petabridge,LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using Akka.Actor;
using Akka.Benchmarker.Actors;
using Akka.Cluster.Sharding;
using Akka.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Akka.Benchmarker.Tests.Utilities;

public interface IWithActorId
{
    string ActorId { get; }
}

/// <summary>
/// Messaging protocol for our tests
/// </summary>
public static class ActorMessages
{
    public sealed record Message(string ActorId) : IWithActorId;

    public sealed record Reply(string ActorId, int MessagesReceivedSoFar) : IWithActorId;
}

public sealed class ActorUnderTest : UntypedActor
{
    private int _messagesReceivedSoFar = 0;

    protected override void OnReceive(object message)
    {
        switch (message)
        {
            case ActorMessages.Message msg:
                _messagesReceivedSoFar++;
                Sender.Tell(new ActorMessages.Reply(msg.ActorId, _messagesReceivedSoFar));
                break;
        }
    }
}

/// <summary>
/// An instance of the <see cref="IActorMessageFlow{TActor}"/> that will send 10 messages to each actor
/// </summary>
public sealed class ActorMessagingFlow : IActorMessageFlow<ActorUnderTest>
{
    public int TotalMessages { get; } = 10;

    public Task ExecuteSingleActorInteractions(IActorRef actorRoot, string actorId, CancellationToken ct)
    {
        foreach (var i in Enumerable.Range(0, TotalMessages - 1))
        {
            actorRoot.Tell(new ActorMessages.Message(actorId));
        }

        return actorRoot.Ask<ActorMessages.Reply>(new ActorMessages.Message(actorId), cancellationToken: ct);
    }
}

public static class AkkaHostingForActorUnderTest
{

    
    public static IServiceCollection ConfigureActorUnderTestServices(this IServiceCollection services,
        IBenchmarkConfiguration configuration)
    {
        return configuration switch
        {
            _ => services
        };
    }

    public static AkkaConfigurationBuilder ConfigureActorUnderTest(this AkkaConfigurationBuilder builder,
        IBenchmarkConfiguration configuration)
    {
        return configuration switch
        {
            _ => ConfigureActorUnderTestLocalSystem(builder, configuration)
        };
    }

    private static AkkaConfigurationBuilder ConfigureActorUnderTestLocalSystem(this AkkaConfigurationBuilder builder,
        IBenchmarkConfiguration configuration)
    {
        IMessageExtractor messageExtractor = HashCodeMessageExtractor.Create(configuration.NumberOfActorSystems * 10, msg =>
        {
            if (msg is IWithActorId withId)
                return withId.ActorId;
            return string.Empty;
        });
        
        return builder
            .WithActors((system, registry, resolver) =>
            {
                var props = Props.Create(() => new ActorUnderTest());
                var propsFunction = new Func<string, Props>(id => props);
                
                // start a generic child per entity parent
                var parent = system.ActorOf(GenericChildPerEntityParent.Props(messageExtractor, propsFunction), "parent");
                
                // register the actor under test
                registry.Register<ActorUnderTest>(parent);
            });
    }
}