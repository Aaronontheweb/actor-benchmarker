using Akka.Benchmarker.Tests.Utilities;
using FluentAssertions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Akka.Hosting.Testkit;
using Akka.Hosting.TestKit.Internals;
using Xunit.Abstractions;

namespace Akka.Benchmarker.Tests;

public class End2EndTests
{
    private readonly ITestOutputHelper _helper;
    
    public End2EndTests(ITestOutputHelper helper)
    {
        _helper = helper;
    }
    
    [Fact(DisplayName = "Should execute a basic benchmark with 1 actor system running locally, under a single configuration")]
    public async Task BasicTestCase()
    {
        // arrange
        var actorSystemConfigurator = new ActorSystemConfigurator<ActorUnderTest>(
            (builder, _, benchmarkConfiguration) => builder.ConfigureActorUnderTest(benchmarkConfiguration),
            _ => Enumerable.Range(0, 100).Select(c => $"actor-{c}"));

        var benchmarkDefinition =
            new ActorBenchmarkDefinition<ActorUnderTest>(actorSystemConfigurator, new ActorMessagingFlow());
        
        // act
        var actorBenchmarks = benchmarkDefinition.CreateBenchmarks().ToList();
        actorBenchmarks.Should().HaveCount(1);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        foreach (var bench in actorBenchmarks)
        {
            var (totalActors, totalMessages) = await bench.Setup(cts.Token);
            _helper.WriteLine("Executing benchmark [{0}] with [{1}] actors and [{2}] messages per actor",
                bench.Configuration.FriendlyConfigurationName, totalActors, totalMessages);
            var startTime = DateTime.UtcNow;
            await bench.Run(cts.Token);
            var endTime = DateTime.UtcNow;
            _helper.WriteLine("Completed benchmark [{0}] in [{1}]ms", bench.Configuration.FriendlyConfigurationName,
                (endTime - startTime).TotalMilliseconds);
            await bench.Teardown(cts.Token);
        }
    }

    [Fact(DisplayName = "Should execute a clustered benchmark with 3 actor systems AND 1 local system as separate benchmarks")]
    public async Task ClusteredAndBasicCase()
    {
        // arrange
        var actorSystemConfigurator = new ActorSystemConfigurator<ActorUnderTest>(
            (builder, _, benchmarkConfiguration) => builder.ConfigureActorUnderTest(benchmarkConfiguration),
            _ => Enumerable.Range(0, 100).Select(c => $"actor-{c}"));

        var benchmarkDefinition =
            new ActorBenchmarkDefinition<ActorUnderTest>(actorSystemConfigurator, new ActorMessagingFlow(), new IBenchmarkConfiguration[]
            {
                DefaultBenchmarkConfiguration.Instance, new ClusteredBenchmarkConfig()
            });
        
        // act
        var actorBenchmarks = benchmarkDefinition.CreateBenchmarks().ToList();
        actorBenchmarks.Should().HaveCount(2);

        foreach (var bench in actorBenchmarks)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var (totalActors, totalMessages) = await bench.Setup(cts.Token);
            _helper.WriteLine("Executing benchmark [{0}] with [{1}] actors and [{2}] messages per actor",
                bench.Configuration.FriendlyConfigurationName, totalActors, totalMessages);
            var startTime = DateTime.UtcNow;
            await bench.Run(cts.Token);
            var endTime = DateTime.UtcNow;
            _helper.WriteLine("Completed benchmark [{0}] in [{1}]ms", bench.Configuration.FriendlyConfigurationName,
                (endTime - startTime).TotalMilliseconds);
            await bench.Teardown(cts.Token);
        }
    }
}