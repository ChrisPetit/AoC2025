namespace Day03.Tests;

public class Puzzle01Tests
{
    [Fact]
    public void SolveTest()
    {
        var input = new[]
        {
            "987654321111111",
            "811111111111119",
            "234234234234278",
            "818181911112111"
        };
        
        var result = Puzzle01.Solve(input);
        Assert.Equal(357, result);
    }
}