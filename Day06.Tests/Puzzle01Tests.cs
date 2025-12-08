namespace Day06.Tests;

public class Puzzle01Tests
{
    [Fact]
    public void SolveTest()
    {
        var input = new[]
        {
            "123 328  51 64 ",
            " 45 64  387 23 ",
            "  6 98  215 314",
            "*   +   *   +  "
        };
        
        var result = Puzzle01.Solve(input);
        Assert.Equal(4277556, result);
    }
}