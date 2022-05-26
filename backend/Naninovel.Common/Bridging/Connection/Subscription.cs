using System;

namespace Naninovel.Bridging;

internal abstract class Subscription
{
    private readonly int id;
    private readonly Action<IMessage> handler;

    protected Subscription (int id, Action<IMessage> handler)
    {
        this.id = id;
        this.handler = handler;
    }

    public void InvokeHandler (IMessage message) => handler.Invoke(message);
    public override bool Equals (object o) => o is Subscription s && s.id == id;
    public override int GetHashCode () => id;
}

internal class Subscription<T> : Subscription where T : IMessage
{
    public Subscription (Action<T> handler)
        : base(handler.GetHashCode(), m => handler.Invoke((T)m)) { }
}
