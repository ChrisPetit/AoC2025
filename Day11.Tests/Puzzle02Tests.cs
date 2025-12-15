namespace Day11.Tests;

public class Puzzle02Tests
{
    [Fact]
    public void SolveTest()
    {
        var input = new[]
        {
            "svr: aaa bbb",
            "aaa: fft",
            "fft: ccc",
            "bbb: tty",
            "tty: ccc",
            "ccc: ddd eee",
            "ddd: hub",
            "hub: fff",
            "eee: dac",
            "dac: fff",
            "fff: ggg hhh",
            "ggg: out",
            "hhh: out"
        };
        
        var result = Puzzle02.Solve(input);
        Assert.Equal(2, result);
    }
}