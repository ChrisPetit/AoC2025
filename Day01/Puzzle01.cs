using System.ComponentModel.Design;

namespace Day01;

/// <summary>
/// Simulates moving a position on a circular track with 100 slots (0-99),
/// starting from position 50, based on a list of movement instructions.
/// Each instruction is a string where the first character is the direction
/// (`R` for right, `L` for left) and the remaining characters form an integer
/// step count. The position is advanced or decremented one step at a time,
/// wrapping from 99 to 0 when moving right and from 0 to 99 when moving left.
/// After each full instruction, if the final position is 0, a counter is
/// incremented. The method returns how many instructions ended at position 0.
/// </summary>
public static class Puzzle01
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
                        
                    }

                    if (start == 0)
                    {
                        zeroCounter++;
                    }
                    break;
                }
                case 'L':
                {
                    for (var i = 0; i < value; i++)
                    {
                        start -= 1;
                        if (start != -1) continue;
                        start = 99;
                        
                    }

                    if (start == 0)
                    {
                        zeroCounter++;
                    }
                    break;
                }
            }
        }

        return zeroCounter;
    }
}