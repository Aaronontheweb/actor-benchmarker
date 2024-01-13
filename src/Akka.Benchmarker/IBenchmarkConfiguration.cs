// -----------------------------------------------------------------------
//  <copyright file="IBenchmarkConfiguration.cs" company="Akka.NET Project">
//      Copyright (C) 2015-2024 Petabridge,LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using Akka.Actor;
using Microsoft.Extensions.Hosting;

namespace Akka.Benchmarker;

/// <summary>
///     Arbitrary interface used to describe the configuration of a given benchmark.
/// </summary>
public interface IBenchmarkConfiguration
{
    /// <summary>
    /// The number of <see cref="ActorSystem"/> instances participating in this benchmark.
    /// </summary>
    int NumberOfActorSystems { get; }
    
    /// <summary>
    ///     The human-readable name for this configuration.
    /// </summary>
    string FriendlyConfigurationName { get; }
    
    /// <summary>
    /// The name of the <see cref="ActorSystem"/>.
    /// </summary>
    string ActorSystemName { get; }

    /// <summary>
    /// Need to start up something like a TestContainer or a Docker container before the benchmark starts? Do it here.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    ValueTask PreHostSetup(CancellationToken ct) => ValueTask.CompletedTask;
    
    /// <summary>
    /// If you need to touch the <see cref="HostBuilder"/> before it starts up, do it here.
    /// </summary>
    /// <param name="hostBuilder">The host builder.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <remarks>
    /// This method is used for doing things like configuring logging or host configuration values.
    /// </remarks>
    ValueTask HostBuilderSetup(HostBuilder hostBuilder, CancellationToken ct) => ValueTask.CompletedTask;
    
    /// <summary>
    /// If we need to do something like manually cluster the <see cref="ActorSystem"/> instances in all <see cref="IHost"/>s before the benchmark starts, do it here.
    /// </summary>
    /// <param name="hosts">All of the hosts participating in this benchmark instance.</param>
    /// <param name="ct">Cancellation token.</param>
    ValueTask PostHostSetup(IHost[] hosts, CancellationToken ct) => ValueTask.CompletedTask;
    
    /// <summary>
    /// Need to tear down something like a TestContainer or a Docker container after the benchmark completes? Do it here.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    ValueTask PostHostTeardown(CancellationToken ct) => ValueTask.CompletedTask;
}

/// <summary>
/// The default configuration if no others are specified
/// </summary>
public sealed class DefaultBenchmarkConfiguration : IBenchmarkConfiguration
{
  
    private DefaultBenchmarkConfiguration()
    {
    }
    
    public static DefaultBenchmarkConfiguration Instance { get; } = new();
    
    public static IBenchmarkConfiguration[] DefaultConfigurations { get; } =
    [
        Instance
    ];

    public int NumberOfActorSystems { get; } = 1;
    public string FriendlyConfigurationName { get; } = "Default";
    
    public string ActorSystemName { get; } = "BenchmarkSys";
}