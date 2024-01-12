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
///     by the time <see cref="ConfigureAction" /> exits.
/// </typeparam>
public sealed class ActorSystemConfigurator<TActor>(
    Action<AkkaConfigurationBuilder, IBenchmarkConfiguration> configureAction,
    Func<IEnumerable<string>, IBenchmarkConfiguration> actorIds,
    Action<IServiceCollection, IBenchmarkConfiguration> configureServices)
    where TActor : ActorBase
{
    /// <summary>
    /// Used to configure the DI container for a given benchmark.
    /// </summary>
    public Action<IServiceCollection, IBenchmarkConfiguration> ConfigureServices { get; } = configureServices;

    /// <summary>
    ///     Configures the underlying ActorSystem for a given benchmark - accepts the Akka.Hosting
    ///     <see cref="AkkaConfigurationBuilder" />
    ///     and a user-defined <see cref="IBenchmarkConfiguration" />.
    /// </summary>
    public Action<AkkaConfigurationBuilder, IBenchmarkConfiguration> ConfigureAction { get; } = configureAction;

    /// <summary>
    ///     Produces a finite set of actor IDs that will be used to create the actors under test.
    /// </summary>
    public Func<IEnumerable<string>, IBenchmarkConfiguration> ActorIds { get; } = actorIds;
}