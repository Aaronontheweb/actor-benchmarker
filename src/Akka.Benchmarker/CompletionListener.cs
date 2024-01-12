// -----------------------------------------------------------------------
//  <copyright file="CompletionListener.cs" company="Akka.NET Project">
//      Copyright (C) 2015-2024 Petabridge,LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using Akka.Actor;
using Akka.Hosting;

namespace Akka.Benchmarker;

/// <summary>
/// Used to determine when the benchmark is complete.
/// </summary>
/// <typeparam name="TActor">The type of actor under benchmark.</typeparam>
public abstract class CompletionListener<TActor>(ActorSystem actorSystem, ActorRegistry actorRegistry)
    where TActor : ActorBase
{
    public ActorSystem ActorSystem { get; } = actorSystem;

    public ActorRegistry ActorRegistry { get; } = actorRegistry;

    /// <summary>
    /// Allows the end user to set up the completion action before the benchmark starts, in order to ensure
    /// that the overhead of setting up the completion action is not included in the benchmark.
    /// </summary>
    /// <param name="ct">Used to time out setups that fail to complete on time.</param>
    /// <returns>A <see cref="ValueTask"/> that will be completed successfully if the setup was successful.</returns>
    public abstract ValueTask Setup(CancellationToken ct);
    
    /// <summary>
    /// Invoked as soon as all of the <see cref="IActorMessageFlow{TActor}"/> invocations have finished - the benchmark
    /// will wait asynchronously for this method to complete before it terminates.
    /// </summary>
    /// <param name="ct">A cancellation token, in the event of a bad benchmark.</param>
    /// <returns>A <see cref="ValueTask"/> that will be completed successfully if the completion condition was met successfully.</returns>
    public abstract ValueTask WaitForCompletion(CancellationToken ct);
}