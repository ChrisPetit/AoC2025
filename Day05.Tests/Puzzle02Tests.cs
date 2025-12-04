namespace Day05.Tests;

public class Puzzle02Tests
{
    [Fact]
    public void SolveTest()
    {
        var input = new[]
        {
            ""
        };
        
        var result = Puzzle01.Solve(input);
        Assert.Equal(0, result);
    }
}