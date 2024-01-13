// -----------------------------------------------------------------------
//  <copyright file="IActorMessageFlow.cs" company="Akka.NET Project">
//      Copyright (C) 2015-2024 Petabridge,LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using Akka.Actor;
using Akka.Hosting;

namespace Akka.Benchmarker;

/// <summary>
/// Used to produce messages that will be sent to each of the actors under test.
/// </summary>
/// <typeparam name="TActor"></typeparam>
public interface IActorMessageFlow<TActor> where TActor : ActorBase
{
    /// <summary>
    /// Used to help compute the relative "size" of the messaging flow
    /// </summary>
    /// <remarks>
    /// If the number of TotalMessages is not predictable: your benchmark is designed poorly. Try again.
    /// </remarks>
    public int TotalMessages { get; }

    /// <summary>
    ///     Create a finite set of messages that will be sent to the actor under test.
    /// </summary>
    /// <param name="actorRoot">The root actor we're going to be messaging, registered inside the <see cref="ActorRegistry"/> as <see cref="TActor"/>.</param>
    /// <param name="actorId">The id of the specific instance of this actor.</param>
    /// <returns>A Task that will complete once all of the messaging interactions with the actor are completed.</returns>
    public Task ExecuteSingleActorInteractions(IActorRef actorRoot, string actorId);
}