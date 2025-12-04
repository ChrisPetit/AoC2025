namespace Day04.Tests;

public class Puzzle01Tests
{
    [Fact]
    public void SolveTest()
    {
        var input = new[]
        {
            "..@@.@@@@.",
            "@@@.@.@.@@",
            "@@@@@.@.@@",
            "@.@@@@..@.",
            "@@.@@@@.@@",
            ".@@@@@@@.@",
            ".@.@.@.@@@",
            "@.@@@.@@@@",
            ".@@@@@@@@.",
            "@.@.@@@.@."
        };
        
        var result = Puzzle01.Solve(input);
        Assert.Equal(13, result);
    }
}