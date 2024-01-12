// -----------------------------------------------------------------------
//  <copyright file="IActorMessageFlow.cs" company="Akka.NET Project">
//      Copyright (C) 2015-2024 Petabridge,LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using Akka.Actor;

namespace Akka.Benchmarker;

/// <summary>
/// Used to produce messages that will be sent to each of the actors under test.
/// </summary>
/// <typeparam name="TActor"></typeparam>
public interface IActorMessageFlow<TActor> where TActor : ActorBase
{
    /// <summary>
    ///     Create a finite set of messages that will be sent to the actor under test.
    /// </summary>
    /// <param name="actorId"></param>
    /// <returns></returns>
    public IEnumerable<object> CreateMessagesForActor(string actorId);
}