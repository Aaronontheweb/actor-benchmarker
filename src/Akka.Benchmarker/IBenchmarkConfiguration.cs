// -----------------------------------------------------------------------
//  <copyright file="IBenchmarkConfiguration.cs" company="Akka.NET Project">
//      Copyright (C) 2015-2024 Petabridge,LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using Akka.Actor;

namespace Akka.Benchmarker;

/// <summary>
///     Arbitrary interface used to describe the configuration of a given benchmark.
/// </summary>
public interface IBenchmarkConfiguration
{
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
    
    public string FriendlyConfigurationName { get; } = "Default";
    
    public string ActorSystemName { get; } = "BenchmarkSys";
}