namespace Day04.Tests;

public class Puzzle02Tests
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

        var result = Puzzle02.Solve(input);
        Assert.Equal(43, result);
    }
}