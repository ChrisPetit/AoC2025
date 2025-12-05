namespace Day04;

/// <summary>
/// Counts how many `@` cells in the grid have fewer than 4 neighboring `@` cells
/// (considering all 8 surrounding positions) and returns that count.
/// </summary>
public static class Puzzle01
{
    public static int Solve(string[]? lines)
    {
        if (lines is null || lines.Length == 0)
            return 0;

        var rows = lines.Length;
        var cols = lines[0].Length;
        var countAccessible = 0;

        // Directions for 8 neighbors: (dr, dc)
        Span<(int dr, int dc)> dirs =
        [
            (-1, -1), (-1, 0), (-1, 1),
            (0,  -1),                ( 0, 1),
            (1,  -1), ( 1, 0), ( 1, 1)
        ];

        for (var r = 0; r < rows; r++)
        {
            var line = lines[r];

            for (var c = 0; c < cols; c++)
            {
                if (line[c] != '@')
                    continue;

                var neighborRolls = 0;

                foreach (var (dr, dc) in dirs)
                {
                    var nr = r + dr;
                    var nc = c + dc;

                    if (nr < 0 || nr >= rows || nc < 0 || nc >= cols)
                        continue;

                    if (lines[nr][nc] == '@')
                        neighborRolls++;
                }

                if (neighborRolls < 4)
                    countAccessible++;
            }
        }

        return countAccessible;
    }
}