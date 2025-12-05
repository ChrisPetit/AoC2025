namespace Day01;

/// <summary>
/// Processes a list of move instructions on a circular track of 100 positions,
/// starting at position 50, and counts how many times position 0 is reached.
/// Each instruction is a string where the first character is the direction
/// ('R' for right, 'L' for left) and the remaining characters represent
/// the number of single-step moves in that direction. The position wraps
/// from 99 to 0 when moving right and from 0 to 99 when moving left.
/// </summary>
public static class Puzzle02
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