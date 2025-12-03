namespace Day03.Tests;

public class Puzzle02Tests
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
    
        var result = Puzzle02.Solve(input);
        Assert.Equal(3121910778619, result);
    }
    
}