namespace Naninovel.Bridging;

internal abstract class Subscription (int id, Action<IMessage> handler)
{
    private readonly int id = id;

    public void InvokeHandler (IMessage message) => handler.Invoke(message);
    public override bool Equals (object? o) => (o as Subscription)?.id == id;
    public override int GetHashCode () => id;
}

internal class Subscription<T> (Action<T> handler)
    : Subscription(handler.GetHashCode(), m => handler.Invoke((T)m)) where T : IMessage;
