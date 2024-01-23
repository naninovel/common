namespace Naninovel.Metadata.Test;

public class ProjectTest
{
    [Fact] // TODO: Change to required when Unity allows and remove this test.
    public void MessagePropsInitializedWithDefaults ()
    {
        Assert.Equal(default, new ValueContext().Type);
        Assert.Equal(default, new ValueContext().SubType);
    }
}
