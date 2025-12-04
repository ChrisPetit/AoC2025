using System;
using System.Collections.Generic;

namespace Day04;

public static class Puzzle02
{
    public static int Solve(string[]? lines)
    {
        if (lines == null || lines.Length == 0)
            return 0;

        var rows = lines.Length;
        var cols = lines[0].Length;

        // Copy to mutable grid
        var grid = new char[rows][];
        for (var r = 0; r < rows; r++)
            grid[r] = lines[r].ToCharArray();

        // Neighbor directions (8-connected)
        ReadOnlySpan<(int dr, int dc)> dirs =
        [
            (-1, -1), (-1, 0), (-1, 1),
            ( 0, -1),                ( 0, 1),
            ( 1, -1), ( 1, 0), ( 1, 1)
        ];

        var totalRemoved = 0;

        while (true)
        {
            var toRemove = new List<(int r, int c)>();

            for (var r = 0; r < rows; r++)
            {
                var row = grid[r];
                for (var c = 0; c < cols; c++)
                {
                    if (row[c] != '@')
                        continue;

                    var neighbors = 0;
                    foreach (var (dr, dc) in dirs)
                    {
                        var nr = r + dr;
                        var nc = c + dc;
                        if (nr < 0 || nr >= rows || nc < 0 || nc >= cols)
                            continue;

                        if (grid[nr][nc] == '@')
                            neighbors++;
                    }

                    if (neighbors < 4)
                        toRemove.Add((r, c));
                }
            }

            if (toRemove.Count == 0)
                break;

            foreach (var (r, c) in toRemove)
            {
                if (grid[r][c] != '@') continue;
                grid[r][c] = '.';
                totalRemoved++;
            }
        }

        return totalRemoved;
    }
}