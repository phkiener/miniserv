namespace MiniServ.Test;

[TestFixture]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public sealed class FileServerOptionsTest
{
    [Test]
    public void EmptyArgs_ParsesToDefaults()
    {
        var options = FileServerOptions.Parse([]);

        Assert.That(options, Is.Not.Null);
        Assert.That(options.ContentRoot, Is.EqualTo(Environment.CurrentDirectory));
        Assert.That(options.Help, Is.False);
        Assert.That(options.Version, Is.False);
        Assert.That(options.Verbose, Is.False);
    }

    [Test]
    [TestCase("-h")]
    [TestCase("--help")]
    public void CanParseHelpFlag(string arg)
    {
        var options = FileServerOptions.Parse([arg]);

        Assert.That(options, Is.Not.Null);
        Assert.That(options.Help, Is.True);
    }

    [Test]
    [TestCase("-v")]
    [TestCase("--version")]
    public void CanParseVersionFlag(string arg)
    {
        var options = FileServerOptions.Parse([arg]);

        Assert.That(options, Is.Not.Null);
        Assert.That(options.Version, Is.True);
    }

    [Test]
    [TestCase("--verbose")]
    public void CanParseVerboseFlag(string arg)
    {
        var options = FileServerOptions.Parse([arg]);

        Assert.That(options, Is.Not.Null);
        Assert.That(options.Verbose, Is.True);
    }

    [Test]
    public void CanParseContentRoot()
    {
        var options = FileServerOptions.Parse(["/foo/bar"]);

        Assert.That(options, Is.Not.Null);
        Assert.That(options.ContentRoot, Is.EqualTo("/foo/bar"));
    }
}
