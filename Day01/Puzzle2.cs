namespace Day01;

public static class Puzzle2
{
    public static int Solve(string[] input)
    {
        var start = 50;
        var zeroCounter = 0;

        foreach (var line in input)
        {
            var direction = line[0];
            var value = int.Parse(line[1..]);
            switch (direction)
            {
                case 'R':
                {
                    for (var i = 0; i < value; i++)
                    {
                        start += 1;
                        if (start != 100) continue;
                        start = 0;
                        zeroCounter++;
                    }
                    
                    break;
                }
                case 'L':
                {
                    for (var i = 0; i < value; i++)
                    {
                        start -= 1;
                        if (start == 0) zeroCounter++;
                        if (start != -1) continue;
                        start = 99;
                    }
                    
                    break;
                }
            }
        }

        return zeroCounter;
    }
}