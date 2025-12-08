namespace Day07;

/// <summary>
/// Simulates downward-moving beams starting from the `S` position in the grid,
/// counting how many times a beam encounters a `^` cell and splits into two
/// horizontal beams (left and right). Lines are padded with `.` to form a
/// rectangular grid; beams move row by row until they leave the grid or reach
/// the bottom, and the total number of splits is returned.
/// </summary>
public static class Puzzle01
{
    public static long Solve(string[]? lines)
    {
        if (lines == null || lines.Length == 0)
            return 0;

        var rows = lines.Length;
        var cols = lines.Max(l => l.Length);

        var grid = new char[rows, cols];
        for (var r = 0; r < rows; r++)
        {
            var line = lines[r];
            for (var c = 0; c < cols; c++)
                grid[r, c] = c < line.Length ? line[c] : '.';
        }

        int sRow = -1, sCol = -1;
        for (var r = 0; r < rows && sRow == -1; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (grid[r, c] != 'S') continue;
                sRow = r;
                sCol = c;
                break;
            }
        }

        if (sRow == -1)
            return 0;

        long splitCount = 0;
        var activeBeams = new HashSet<int> { sCol };

        for (var r = sRow; r < rows; r++)
        {
            var nextBeams = new HashSet<int>();
            foreach (var c in activeBeams)
            {
                if (c < 0 || c >= cols)
                    continue;

                var cell = grid[r, c];

                if (cell == '^')
                {
                    splitCount++;
                    // New beams start left and right on the same row
                    nextBeams.Add(c - 1);
                    nextBeams.Add(c + 1);
                }
                else // '.' or 'S'
                {
                    // Beam continues straight down
                    nextBeams.Add(c);
                }
            }

            activeBeams = nextBeams;
        }

        return splitCount;
    }
}