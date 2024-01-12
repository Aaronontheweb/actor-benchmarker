namespace Akka.Benchmarker.App;

public class TransformerActor : ReceiveActor
{
    public TransformerActor()
    {
        Receive<string>(str =>
        {
            Sender.Tell(str.ToUpperInvariant());
        });
    }
}