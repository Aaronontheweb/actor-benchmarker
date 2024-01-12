// -----------------------------------------------------------------------
//  <copyright file="ActorBenchmarkDefinition.cs" company="Akka.NET Project">
//      Copyright (C) 2013-2024 .NET Foundation <https://github.com/akkadotnet/akka.net>
// </copyright>
// -----------------------------------------------------------------------

using Akka.Actor;
using Akka.Hosting;

namespace Akka.Benchmarker;

/// <summary>
/// Describes the sources of 
/// </summary>
public sealed class ActorBenchmarkDefinition
{
}

/// <summary>
/// Arbitrary interface used to describe the configuration of a given benchmark.
/// </summary>
public interface IBenchmarkConfiguration
{
    /// <summary>
    /// The human-readable name for this configuration.
    /// </summary>
    string FriendlyConfigurationName { get; }
}

/// <summary>
/// Used to configure the underlying ActorSystem for a given benchmark
/// </summary>
/// <typeparam name="TActor">The type of actor under test - must be made available inside the <see cref="ActorRegistry"/> by the time <see cref="ConfigureAction"/> exits.</typeparam>
public sealed class ActorSystemConfigurator<TActor>(
    Action<AkkaConfigurationBuilder, IBenchmarkConfiguration> configureAction,
    Func<IEnumerable<string>> actorIds)
    where TActor : ActorBase
{
    /// <summary>
    /// Configures the underlying ActorSystem for a given benchmark - accepts the Akka.Hosting <see cref="AkkaConfigurationBuilder"/>
    /// and a user-defined <see cref="IBenchmarkConfiguration"/>.
    /// </summary>
    public Action<AkkaConfigurationBuilder, IBenchmarkConfiguration> ConfigureAction { get; } = configureAction;

    /// <summary>
    /// Produces a finite set of actor IDs that will be used to create the actors under test.
    /// </summary>
    public Func<IEnumerable<string>> ActorIds { get; } = actorIds;
}

public interface IActorMessageFlow<TActor> where TActor:ActorBase
{

    /// <summary>
    /// Create a finite set of messages that will be sent to the actor under test.
    /// </summary>
    /// <param name="actorId"></param>
    /// <returns></returns>
    public IEnumerable<object> CreateMessagesForActor(string actorId);
}