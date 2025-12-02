namespace Day02.Tests;

public class Puzzle01Tests
{
    [Fact]
    public void SolveTest()
    {
        var input =
            "11-22,95-115,998-1012,1188511880-1188511890,222220-222224,1698522-1698528,446443-446449,38593856-38593862,565653-565659,824824821-824824827,2121212118-2121212124";
        
        var lines = input.Split(',');
        var result = Puzzle02.Solve(lines);
        Assert.Equal(4174379265, result);
    }
}
