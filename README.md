# actor-benchmarker
Tooling for generating automated [Akka.NET](https://getakka.net/) end-to-end benchmarks for actors. Think big, "macro" benchmarks - not your traditional Benchmark.NET micro-benchmarks you often see on Twitter et al.

> ![NOTE]
> This library is an experiment and not meant for production use.

## Benefits of Akka.Benchmarker

Why use this library?

1. **Eliminates Boilerplate** - we make it very easy to parameterize your macro benchmarks with a variety of different `ActorSystem` configuration options. We make sure to eliminate setup overhead from your benchmarks AND we cleanly tear down dependencies afterwards.
2. **Forces Good Practices** - these benchmark methodologies are designed by the team that build and maintains Akka.NET itself; we've had practice doing this type of benchmarking for nearly 10 years. Only now, we've codified everything into a streamlined set of abstractions that are very accessible and user-friendly.
3. **No HOCON** - everything in Akka.Benchmarker is configured with [Akka.Hosting](https://github.com/akkadotnet/Akka.Hosting). No HOCON is required.
4. **Parameterizable** - it's very easy to test your actors under a variety of different configurations (i.e. different `ActorSystem` settings, serializers, Akka.Persistence back-ends).

## Designing a Benchmark

The idea behind Akka.Benchmarker (the `namespace` for this library) is to choose the following:

1. **Code Under Test**: choose a single "head actor" that represents a set of related entities - in Akka.Cluster parlance this would typically be a [`ShardRegion`](https://getakka.net/articles/clustering/cluster-sharding.html). In local Akka.NET this might be the aggregate root / parent actor for something like the [child-per-entity pattern](https://petabridge.com/blog/top-akkadotnet-design-patterns/). These actors typically perform complex work; have state; and are where the real "business" of your application is conducted. These actors might also interact with other dependent actors - that is totally fine: **we want all of that overhead and data included in our benchmark**.
2. **Infrastructure Under Test**: need to use SQL Server, Redis, or MongoDb as part of your test configuration? Akka.Benchmarker will allow you to fire up and tear down external infrastructure dependencies like [Test Conatiners](https://testcontainers.com/). Again, **these are meant to be end to end benchmarks - real overhead is what we are trying to measure**.
3. **Clusters or Single Actor Systems**: the `IBenchmarkConfiguration` construct in Akka.Benchmarker makes it very easy to say "give me a cluster of 4 nodes" or "run as a single `ActorSystem`" for your benchmark. Implement your own `IBenchmarkConfiguration` types to help express this. You can run multiple `IBenchmarkConfiguration`s for the same code and infrastructure under test.

## Implementing Your Design

```
TODO: going to test this with some live applications first
```

Copyright 2015-2024 Aaron Stannard [<aaronstannard.com>](https://aaronstannard.com/)