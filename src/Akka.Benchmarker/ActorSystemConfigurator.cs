// -----------------------------------------------------------------------
//  <copyright file="ActorSystemConfigurator.cs" company="Akka.NET Project">
//      Copyright (C) 2015-2024 Petabridge,LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using Akka.Actor;
using Akka.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Akka.Benchmarker;

/// <summary>
///     Used to configure the underlying ActorSystem for a given benchmark
/// </summary>
/// <typeparam name="TActor">
///     The type of actor under test - must be made available inside the <see cref="ActorRegistry" />
///     by the time <see cref="ConfigureActorSystem" /> exits.
/// </typeparam>
public sealed class ActorSystemConfigurator<TActor>(
    Action<AkkaConfigurationBuilder, IServiceProvider, IBenchmarkConfiguration> configureActorSystem,
    Func<IBenchmarkConfiguration, IEnumerable<string>> actorIds,
    Action<IServiceCollection, IBenchmarkConfiguration>? configureServices = null)
    where TActor : ActorBase
{
    /// <summary>
    /// Used to configure the DI container for a given benchmark.
    /// </summary>
    public Action<IServiceCollection, IBenchmarkConfiguration> ConfigureServices { get; } = configureServices ?? ((_, _) => {});

    /// <summary>
    ///     Configures the underlying ActorSystem for a given benchmark - accepts the Akka.Hosting
    ///     <see cref="AkkaConfigurationBuilder" />
    ///     and a user-defined <see cref="IBenchmarkConfiguration" />.
    /// </summary>
    public Action<AkkaConfigurationBuilder, IServiceProvider, IBenchmarkConfiguration> ConfigureActorSystem { get; } = configureActorSystem;

    /// <summary>
    ///     Produces a finite set of actor IDs that will be used to create the actors under test.
    /// </summary>
    public Func<IBenchmarkConfiguration, IEnumerable<string>> ActorIds { get; } = actorIds;
}