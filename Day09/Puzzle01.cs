namespace Day09;

/// <summary>
/// Calculates the maximum rectangular area (axis-aligned) that can be formed
/// from all pairs of 2D points given as `"x,y"` strings. The rectangle sides
/// are aligned to the axes and include both end coordinates.
/// </summary>
public static class Puzzle01
{
    public static long Solve(string[]? lines)
    {
        if (lines == null)
            return 0;

        var points = new List<(long X, long Y)>();
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var parts = line.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
                throw new FormatException($"Invalid coordinate: {line}");

            points.Add((long.Parse(parts[0]), long.Parse(parts[1])));
        }

        if (points.Count < 2)
            return 0;

        long maxArea = 0;
        for (var i = 0; i < points.Count - 1; i++)
        {
            for (var j = i + 1; j < points.Count; j++)
            {
                var width = Math.Abs(points[i].X - points[j].X) + 1;
                var height = Math.Abs(points[i].Y - points[j].Y) + 1;
                var area = width * height;
                if (area > maxArea)
                    maxArea = area;
            }
        }

        return maxArea;
    }
}