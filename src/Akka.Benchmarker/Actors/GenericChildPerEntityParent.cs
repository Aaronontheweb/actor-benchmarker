// -----------------------------------------------------------------------
//  <copyright file="GenericChildPerEntityParent.cs" company="Akka.NET Project">
//      Copyright (C) 2015-2024 Petabridge,LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using Akka.Actor;
using Akka.Cluster.Sharding;

namespace Akka.Benchmarker.Actors;

/// <summary>
/// A generic "child per entity" parent actor.
/// </summary>
/// <remarks>
/// Intended for simplifying unit tests where we don't want to use Akka.Cluster.Sharding.
/// </remarks>
public sealed class GenericChildPerEntityParent(IMessageExtractor extractor, Func<string, Props> propsFactory)
    : UntypedActor
{
    public static Props Props(IMessageExtractor extractor, Func<string, Props> propsFactory)
    {
        return Actor.Props.Create(() => new GenericChildPerEntityParent(extractor, propsFactory));
    }
    
    /*
     * Re-use Akka.Cluster.Sharding's infrastructure here to keep things simple.
     */

    protected override void OnReceive(object message)
    {
        var entityId = extractor.EntityId(message);
        if (string.IsNullOrEmpty(entityId)) 
            return;
        
        Context.Child(entityId).GetOrElse(() => Context.ActorOf(propsFactory(entityId), entityId))
            .Forward(extractor.EntityMessage(message));
    }
}