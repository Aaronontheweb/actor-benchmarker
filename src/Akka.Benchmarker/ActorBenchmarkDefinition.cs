// -----------------------------------------------------------------------
//  <copyright file="ActorBenchmarkDefinition.cs" company="Akka.NET Project">
//      Copyright (C) 2015-2024 Petabridge,LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using Akka.Actor;

namespace Akka.Benchmarker;

/// <summary>
///     Describes the sources of
/// </summary>
public sealed class ActorBenchmarkDefinition<TActor>(
    ActorSystemConfigurator<TActor> configurator,
    IActorMessageFlow<TActor> messageFlow,
    Func<CompletionListener<TActor>> completionListenerFactory,
    IBenchmarkConfiguration[]? configurations = null)
    where TActor : ActorBase
{
    /// <summary>
    /// All of the possible configurations for this benchmark.
    /// </summary>
    public IBenchmarkConfiguration[] Configurations { get; } = configurations ?? Array.Empty<IBenchmarkConfiguration>();

    /// <summary>
    /// Used to configure the underlying ActorSystem for a given benchmark - accepts the Akka.Hosting
    /// </summary>
    public ActorSystemConfigurator<TActor> Configurator { get; } = configurator;

    public IActorMessageFlow<TActor> MessageFlow { get; } = messageFlow;

    public Func<CompletionListener<TActor>> CompletionListenerFactory { get; } = completionListenerFactory;
}