public class SmokeTests
{
    [Xunit.Fact]
    public void OfflineDependenciesWork() => Xunit.Assert.Equal(4, 2 + 2);
}
