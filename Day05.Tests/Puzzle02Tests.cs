namespace Day05.Tests;

public class Puzzle02Tests
{
    [Fact]
    public void SolveTest()
    {
        var input = new[]
        {
            "3-5",
            "10-14",
            "16-20",
            "12-18",
            "",
            "1",
            "5",
            "8",
            "11",
            "17",
            "32"
        };
        
        var result = Puzzle02.Solve(input);
        Assert.Equal(14, result);
    }
}