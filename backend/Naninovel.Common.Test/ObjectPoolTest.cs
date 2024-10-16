namespace Naninovel.Test;

public class ObjectPoolTest
{
    private class MockObject;

    [Fact]
    public void WhenNoFreeObjectsFactorsNew ()
    {
        var pool = CreatePool();
        var rentA = pool.Rent();
        var rentB = pool.Rent();
        Assert.NotSame(rentA, rentB);
    }

    [Fact]
    public void ReusesReturnedObjects ()
    {
        var pool = CreatePool();
        var rentA = default(MockObject);
        var rentB = default(MockObject);
        using (pool.Rent(out rentA)) { }
        using (pool.Rent(out rentB)) { }
        Assert.Same(rentA, rentB);
    }

    [Fact]
    public void NotReusesAfterDrop ()
    {
        var pool = CreatePool();
        var rentA = default(MockObject);
        var rentB = default(MockObject);
        using (pool.Rent(out rentA)) { }
        pool.Drop();
        using (pool.Rent(out rentB)) { }
        Assert.NotSame(rentA, rentB);
    }

    [Fact]
    public void ReusesMultipleReturnedObjects ()
    {
        var pool = CreatePool(new() { MaxSize = 2 });
        var rentA1 = pool.Rent();
        var rentB1 = pool.Rent();
        pool.Return(rentA1);
        pool.Return(rentB1);
        var rentA2 = pool.Rent();
        var rentB2 = pool.Rent();
        Assert.Same(rentA1, rentA2);
        Assert.Same(rentB1, rentB2);
    }

    [Fact]
    public void NotReusesOnOverflow ()
    {
        var pool = CreatePool(new() { MaxSize = 1 });
        var rentA1 = pool.Rent();
        var rentB1 = pool.Rent();
        pool.Return(rentA1);
        pool.Return(rentB1);
        var rentA2 = pool.Rent();
        var rentB2 = pool.Rent();
        Assert.Same(rentA1, rentA2);
        Assert.NotSame(rentB1, rentB2);
    }

    [Fact]
    public void CountsObjectsCorrectly ()
    {
        var pool = CreatePool();
        Assert.Equal(0, pool.Total);
        Assert.Equal(0, pool.Rented);
        Assert.Equal(0, pool.Available);

        var rentedA = pool.Rent();
        Assert.Equal(1, pool.Total);
        Assert.Equal(1, pool.Rented);
        Assert.Equal(0, pool.Available);

        var rentedB = pool.Rent();
        Assert.Equal(2, pool.Total);
        Assert.Equal(2, pool.Rented);
        Assert.Equal(0, pool.Available);

        pool.Return(rentedA);
        Assert.Equal(2, pool.Total);
        Assert.Equal(1, pool.Rented);
        Assert.Equal(1, pool.Available);

        pool.Return(rentedB);
        Assert.Equal(2, pool.Total);
        Assert.Equal(0, pool.Rented);
        Assert.Equal(2, pool.Available);
    }

    [Fact]
    public void InvokesCallbacks ()
    {
        var rented = new List<MockObject>();
        var returned = new List<MockObject>();
        var dropped = new List<MockObject>();
        var pool = CreatePool(new() {
            MaxSize = 2,
            OnRent = rented.Add,
            OnReturn = returned.Add,
            OnDrop = dropped.Add
        });
        var rentA = pool.Rent();
        var rentB = pool.Rent();
        var rentC = pool.Rent();
        pool.Return(rentA);
        pool.Return(rentB);
        pool.Return(rentC);
        Assert.Equal([rentA, rentB, rentC], rented);
        Assert.Equal([rentA, rentB, rentC], returned);
        Assert.Equal([rentC], dropped);
        pool.Drop();
        Assert.Equal([rentC, rentB, rentA], dropped);
    }

    [Fact]
    public void DropsPoolOnDispose ()
    {
        var pool = CreatePool();
        _ = pool.Rent();
        Assert.Equal(1, pool.Total);
        pool.Dispose();
        Assert.Equal(0, pool.Total);
    }

    [Fact]
    public void CollectionPoolsClearOnReturn ()
    {
        var rentA = CollectionPool<List<MockObject>, MockObject>.Rent();
        rentA.Add(new());
        CollectionPool<List<MockObject>, MockObject>.Return(rentA);
        Assert.Empty(rentA);
    }

    [Fact]
    public void CollectionPoolTrivia ()
    {
        var rentA1 = ListPool<MockObject>.Rent();
        ListPool<MockObject>.Return(rentA1);
        var rentA2 = ListPool<MockObject>.Rent();
        Assert.Same(rentA1, rentA2);

        var rentB1 = default(List<MockObject>);
        using (ListPool<MockObject>.Rent(out rentB1))
            Assert.NotSame(rentA1, rentB1);
        var rentB2 = ListPool<MockObject>.Rent();
        Assert.Same(rentB1, rentB2);

        ListPool<MockObject>.Return(rentA2);
        ListPool<MockObject>.Return(rentB2);
        ListPool<MockObject>.Drop();
        var rentC = ListPool<MockObject>.Rent();
        Assert.NotSame(rentA2, rentC);
        Assert.NotSame(rentB2, rentC);
    }

    [Fact]
    public void GenericPoolTrivia ()
    {
        var rentA1 = GenericPool<MockObject>.Rent();
        GenericPool<MockObject>.Return(rentA1);
        var rentA2 = GenericPool<MockObject>.Rent();
        Assert.Same(rentA1, rentA2);

        var rentB1 = default(MockObject);
        using (GenericPool<MockObject>.Rent(out rentB1))
            Assert.NotSame(rentA1, rentB1);
        var rentB2 = GenericPool<MockObject>.Rent();
        Assert.Same(rentB1, rentB2);

        GenericPool<MockObject>.Return(rentA2);
        GenericPool<MockObject>.Return(rentB2);
        GenericPool<MockObject>.Drop();
        var rentC = GenericPool<MockObject>.Rent();
        Assert.NotSame(rentA2, rentC);
        Assert.NotSame(rentB2, rentC);
    }

    [Fact]
    public void CanDisablePooling ()
    {
        ObjectPool.PoolingEnabled = false;
        var pool = CreatePool();
        var rentA = default(MockObject);
        var rentB = default(MockObject);
        using (pool.Rent(out rentA)) { }
        using (pool.Rent(out rentB)) { }
        Assert.NotSame(rentA, rentB);
        ObjectPool.PoolingEnabled = true;
    }

    private ObjectPool<MockObject> CreatePool (ObjectPool<MockObject>.Options options = null)
    {
        return new ObjectPool<MockObject>(() => new(), options);
    }
}
