namespace Day11.Tests;

public class Puzzle01Tests
{
    [Fact]
    public void SolveTest()
    {
        var input = new[]
        {
            "aaa: you hhh",
            "you: bbb ccc",
            "bbb: ddd eee",
            "ccc: ddd eee fff",
            "ddd: ggg",
            "eee: out",
            "fff: out",
            "ggg: out",
            "hhh: ccc fff iii",
            "iii: out"
        };
        
        var result = Puzzle01.Solve(input);
        Assert.Equal(5, result);
    }
}