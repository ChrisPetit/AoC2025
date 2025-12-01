namespace Day01.Tests;

public class Puzzle2Tests
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
        
        var result = Puzzle2.Solve(input);
        Assert.Equal(6, result);
    }
}