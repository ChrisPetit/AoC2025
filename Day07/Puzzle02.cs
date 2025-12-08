namespace Day07;

/// <summary>
/// Simulates particles falling from the start position `S` through a grid,
/// where each row is processed from top to bottom and each column position
/// tracks how many timelines (particles) occupy it. A particle normally
/// continues straight down through `.` or `S`. When a particle encounters
/// a `^` cell, it splits into two particles that move to the left and right
/// columns on the next row, duplicating its timeline count. Particles that
/// move outside the grid columns are counted as finished immediately. After
/// processing all rows, the method returns the total number of timelines,
/// including those that exited the grid early and those that ended in the
/// last row.
/// </summary>
public static class Puzzle02
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
                if (grid[r, c] == 'S')
                {
                    sRow = r;
                    sCol = c;
                    break;
                }
            }
        }

        if (sRow == -1)
            return 0;

        var activeParticles = new Dictionary<int, long> { { sCol, 1L } };
        long totalTimelines = 0;

        for (var r = sRow; r < rows; r++)
        {
            var nextParticles = new Dictionary<int, long>();
            foreach (var particle in activeParticles)
            {
                var c = particle.Key;
                var timelines = particle.Value;

                if (c < 0 || c >= cols)
                {
                    totalTimelines += timelines;
                    continue;
                }

                var cell = grid[r, c];

                if (cell == '^')
                {
                    // Particle splits, creating two new paths.
                    // Add timelines to the left particle.
                    var leftCol = c - 1;
                    nextParticles.TryGetValue(leftCol, out var leftCount);
                    nextParticles[leftCol] = leftCount + timelines;

                    // Add timelines to the right particle.
                    var rightCol = c + 1;
                    nextParticles.TryGetValue(rightCol, out var rightCount);
                    nextParticles[rightCol] = rightCount + timelines;
                }
                else // '.' or 'S'
                {
                    // Particle continues straight down.
                    nextParticles.TryGetValue(c, out var currentCount);
                    nextParticles[c] = currentCount + timelines;
                }
            }
            activeParticles = nextParticles;
        }

        // Add timelines for any particles that finished within the grid boundaries.
        foreach (var particle in activeParticles)
        {
            totalTimelines += particle.Value;
        }

        return totalTimelines;
    }
}