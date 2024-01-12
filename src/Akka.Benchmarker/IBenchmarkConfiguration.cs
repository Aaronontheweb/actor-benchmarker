// -----------------------------------------------------------------------
//  <copyright file="IBenchmarkConfiguration.cs" company="Akka.NET Project">
//      Copyright (C) 2015-2024 Petabridge,LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

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
}