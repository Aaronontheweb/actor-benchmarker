// -----------------------------------------------------------------------
//  <copyright file="ActorBenchmark.cs" company="Akka.NET Project">
//      Copyright (C) 2015-2024 Petabridge,LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using Akka.Actor;
using Akka.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Akka.Benchmarker;

public interface IActorBenchmark
{
    IBenchmarkConfiguration Configuration { get; }
    
    Task<(int totalActors, int messagesPerActor)> Setup(CancellationToken ct);
    
    Task Run(CancellationToken ct);
    
    Task Teardown(CancellationToken ct);
}

/// <summary>
/// Runs a single benchmark for a given <see cref="ActorSystemConfigurator{TActor}"/> and <see cref="IActorMessageFlow{TActor}"/>.
/// </summary>
/// <typeparam name="TActor">The head actor that is under test</typeparam>
public sealed class ActorBenchmark<TActor>(
    IBenchmarkConfiguration configuration,
    IActorMessageFlow<TActor> messageFlow,
    ActorSystemConfigurator<TActor> configurator)
    : IActorBenchmark
    where TActor : ActorBase
{
    public IBenchmarkConfiguration Configuration { get; } = configuration;

    public IActorMessageFlow<TActor> MessageFlow { get; } = messageFlow;

    public ActorSystemConfigurator<TActor> Configurator { get; } = configurator;

    public CompletionListener<TActor> CompletionListener { get; private set; } = null!;
    
    public IHost? Host { get; private set; } = null;
    
    /// <summary>
    /// The live <see cref="TActor"/> instance inside the <see cref="ActorSystem"/>.
    /// </summary>
    public IActorRef? HeadActor { get; private set; }
    
    private string[] ActorIds { get; set; } = Array.Empty<string>();
    
    public async Task<(int totalActors, int messagesPerActor)> Setup(CancellationToken ct)
    {
        // first, need to do any pre-host setup
        await Configuration.PreHostSetup(ct);
        
        var hostBuilder = new HostBuilder()
            .ConfigureServices(services =>
            {
                // configure DI services, if necessary
                Configurator.ConfigureServices(services, Configuration);
                services.AddAkka(Configuration.ActorSystemName, (builder, provider) =>
                {
                    Configurator.ConfigureActorSystem(builder, provider, Configuration);
                });
            });
        
        Host = hostBuilder.Build();
        
        // validate that the service can start up correctly
        await Host.StartAsync(ct);
        
        // ensure that TActor is available in the ActorRegistry
        var actorRegistry = Host.Services.GetRequiredService<ActorRegistry>();

        // if this times out or fails, the benchmark will not run (by design)
        HeadActor = await actorRegistry.GetAsync<TActor>(ct);
        
        // compute the total number of actors and messages per actor
        ActorIds = Configurator.ActorIds(Configuration).ToArray();
        
        // assert that the number of actors is greater than 0
        if (ActorIds.Length == 0)
            throw new ArgumentOutOfRangeException(nameof(Configurator), "Configurator.ActorIds must yield at least one actor ID");
        
        var messagesPerActor = MessageFlow.CreateMessagesForActor(ActorIds.First()).Count();
        
        // assert that the number of messages per actor is greater than 0
        if (messagesPerActor == 0)
            throw new ArgumentOutOfRangeException(nameof(MessageFlow), "Must have at least one message per actor");
        
        // initialize the completion listener
        await CompletionListener.Setup(ct);
        
        // done
        return (ActorIds.Length, messagesPerActor);
    }
    
    /// <summary>
    /// Executes the benchmark.
    /// </summary>
    /// <param name="ct">Cancellation token, in case of time out</param>
    public async Task Run(CancellationToken ct)
    {
        for(var i = 0; i < ActorIds.Length; i++)
        {
            var actorId = ActorIds[i];
            var messages = MessageFlow.CreateMessagesForActor(actorId);
            foreach (var message in messages)
            {
                HeadActor!.Tell(message, ActorRefs.NoSender);
            }
        }
        
        await CompletionListener.WaitForCompletion(ct);
    }
    
    public async Task Teardown(CancellationToken ct)
    {
        try
        {
            await Host!.StopAsync(ct);
        }
        catch (Exception ex)
        {
            // TODO: add logging here
        }
        finally
        {
            await Configuration.PostHostTeardown(ct);
        }
    }
}