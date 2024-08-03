namespace Naninovel.Metadata.Test;

public class ConstantEvaluatorTest
{
    private string inspectedScriptPath = "";
    private ConstantEvaluator.GetParamValue getParamValue = (id, idx) => null;

    [Fact]
    public void WhenNullOrEmptyReturnsEmpty ()
    {
        Assert.Empty(Evaluate(null));
        Assert.Empty(Evaluate(""));
    }

    [Fact]
    public void WhenDoesntContainExpressionsReturnsUnmodified ()
    {
        Assert.Equal(new[] { "foo" }, Evaluate("foo"));
    }

    [Fact]
    public void CanResolveScript ()
    {
        inspectedScriptPath = "foo";
        Assert.Equal(new[] { "foo" }, Evaluate("{$Script}"));
    }

    [Fact]
    public void DoesntModifyContentOutsideOfExpression ()
    {
        inspectedScriptPath = "bar";
        Assert.Equal(new[] { "foo/bar" }, Evaluate("foo/{$Script}"));
    }

    [Fact]
    public void CanInjectParamValues ()
    {
        getParamValue = (id, idx) => id == "1" ? "foo" : "bar";
        Assert.Equal(new[] { "foo/bar" }, Evaluate("{:1}/{:2}"));
    }

    [Fact]
    public void WhenParamValueIsNullEmptyReturned ()
    {
        getParamValue = (id, idx) => null;
        Assert.Empty(Evaluate("{:foo}"));
    }

    [Fact]
    public void CanUseNullCoalescing ()
    {
        inspectedScriptPath = "bar";
        getParamValue = (id, idx) => id == "foo" ? "foo" : null;
        Assert.Equal(new[] { "foobar" }, Evaluate("{:foo??:bar}{:bar??$Script}"));
    }

    [Fact]
    public void CanGetEnumerableParamValue ()
    {
        inspectedScriptPath = "nya";
        getParamValue = (id, idx) => idx == 0 ? "foo" : idx == 1 ? "bar" : null;
        Assert.Equal(new[] { "foo/bar/nya" }, Evaluate("{:foo[0]}/{:bar[1]}/{:nya[2]??$Script}"));
    }

    [Fact]
    public void CanConcatenate ()
    {
        inspectedScriptPath = "nya";
        getParamValue = (id, idx) => idx == 0 ? "foo" : idx == 1 ? "bar" : null;
        Assert.Equal(new[] { "foo/bar/nya", "nya/foo", "foo" }, Evaluate("{:foo[0]}/{:bar[1]}/{:nya[2]??$Script}+{$Script}/foo+{:foo[0]}"));
    }

    [Fact]
    public void WhenIncorrectExpressionReturnsEmpty ()
    {
        Assert.Empty(Evaluate("{:foo??bar}"));
    }

    private IReadOnlyList<string> Evaluate (string expression)
    {
        return ConstantEvaluator.EvaluateNames(expression, inspectedScriptPath, getParamValue);
    }
}
