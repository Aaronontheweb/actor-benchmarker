// -----------------------------------------------------------------------
//  <copyright file="ActorBenchmarkDefinition.cs" company="Akka.NET Project">
//      Copyright (C) 2015-2024 Petabridge,LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using Akka.Actor;

namespace Akka.Benchmarker;

/// <summary>
///    Used to create a set of <see cref="IActorBenchmark" /> instances for a given benchmark.
/// </summary>
public interface IBenchmarkDefinition
{
    IEnumerable<IActorBenchmark> CreateBenchmarks();
}

/// <summary>
///     Describes the sources of
/// </summary>
public sealed class ActorBenchmarkDefinition<TActor>(
    ActorSystemConfigurator<TActor> configurator,
    IActorMessageFlow<TActor> messageFlow,
    IBenchmarkConfiguration[]? configurations = null) : IBenchmarkDefinition
    where TActor : ActorBase
{
    /// <summary>
    /// All of the possible configurations for this benchmark.
    /// </summary>
    public IBenchmarkConfiguration[] Configurations { get; } = configurations ?? DefaultBenchmarkConfiguration.DefaultConfigurations;

    /// <summary>
    /// Used to configure the underlying ActorSystem for a given benchmark - accepts the Akka.Hosting
    /// </summary>
    public ActorSystemConfigurator<TActor> Configurator { get; } = configurator;

    public IActorMessageFlow<TActor> MessageFlow { get; } = messageFlow;
    
    public IEnumerable<IActorBenchmark> CreateBenchmarks()
    {
        foreach (var config in Configurations)
        {
            yield return new ActorBenchmark<TActor>(config, MessageFlow, Configurator);
        }
    }
}