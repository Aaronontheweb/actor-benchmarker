// -----------------------------------------------------------------------
//  <copyright file="ActorUnderTestBenchmarks.cs" company="Akka.NET Project">
//      Copyright (C) 2015-2024 Petabridge,LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using Akka.Benchmarker.Tests.Utilities;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Loggers;

namespace Akka.Benchmarker.Samples.BDN;

[SimpleJob(RunStrategy.Monitoring, launchCount: 10, warmupCount: 10)]
public class ActorUnderTestBenchmarks
{
    private static readonly ILogger Logger = ConsoleLogger.Default;

    [ParamsSource(nameof(Configurations))] public IBenchmarkConfiguration Configuration { get; set; }

    private IActorBenchmark? _benchmark;

    [GlobalSetup]
    public async Task GlobalSetup()
    {
        var actorSystemConfigurator =
            new ActorSystemConfigurator<ActorUnderTest>(
                (builder, _, benchmarkConfiguration) => builder.ConfigureActorUnderTest(benchmarkConfiguration),
                _ => Enumerable.Range(0, 100).Select(c => $"actor-{c}"));

        var benchmarkDefinition =
            new ActorBenchmarkDefinition<ActorUnderTest>(actorSystemConfigurator, new ActorMessagingFlow(),
            [
                Configuration
            ]);

        var allBenchmarks = benchmarkDefinition.CreateBenchmarks();

        // should only be a single benchmark defined
        _benchmark = allBenchmarks.Single();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        (int actorCount, int msgCount) = await _benchmark.Setup(cts.Token);
        Logger.WriteLine(LogKind.Header,
            $"Executing benchmark [{_benchmark.Configuration.FriendlyConfigurationName}] with [{actorCount}] actors and [{msgCount}] messages per actor");
    }

    private CancellationTokenSource? _cts;

    [IterationSetup]
    public void IterationSetup()
    {
        _cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
    }

    [Benchmark]
    public async Task Benchmark()
    {
        if (_cts != null)
        {
            await _benchmark!.Run(_cts.Token);
        }
    }

    [IterationCleanup]
    public void IterationCleanup()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    [GlobalCleanup]
    public async Task GlobalTeardown()
    {
        using CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
        if (_benchmark != null) await _benchmark.Teardown(cts.Token);
    }

    public static IEnumerable<IBenchmarkConfiguration> Configurations()
    {
        yield return DefaultBenchmarkConfiguration.Instance; // local, 1 ActorSystem 
        yield return new ClusteredBenchmarkConfig(); // clustered, 1 ShardRegion spread across 3 ActorSystems
    }
}