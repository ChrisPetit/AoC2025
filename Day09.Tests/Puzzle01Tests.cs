namespace Day09.Tests;

public class Puzzle01Tests
{
    [Fact]
    public void SolveTest()
    {
        var input = new[]
        {
            "7,1",
            "11,1",
            "11,7",
            "9,7",
            "9,5",
            "2,5",
            "2,3",
            "7,3"
        };
        
        var result = Puzzle01.Solve(input);
        Assert.Equal(50, result);
    }
}