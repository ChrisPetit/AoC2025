namespace Day01.Tests;

public class Puzzle1Tests
{
    [Fact]
    public void SolveTest()
    {
        var input = new[]
        {
            "L68",
            "L30",
            "R48",
            "L5",
            "R60",
            "L55",
            "L1",
            "L99",
            "R14",
            "L82"
        };
        
        var result = Puzzle01.Solve(input);
        Assert.Equal(3, result);
    }
}