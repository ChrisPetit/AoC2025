namespace Day05;

public static class Puzzle02
{
    public static long Solve(string[] lines)
    {
        var ranges = new List<(long Start, long End)>();
        var inRanges = true;

        foreach (var raw in lines)
        {
            var line = raw.Trim();

            if (line.Length == 0)
            {
                inRanges = false;
                continue;
            }

            if (!inRanges)
                continue;

            var parts = line.Split('-');
            if (parts.Length != 2)
                continue;

            var start = long.Parse(parts[0]);
            var end = long.Parse(parts[1]);
            if (start > end)
                (start, end) = (end, start);

            ranges.Add((start, end));
        }

        if (ranges.Count == 0)
            return 0;

        ranges.Sort((a, b) => a.Start.CompareTo(b.Start));

        var merged = new List<(long Start, long End)>();
        var current = ranges[0];

        for (var i = 1; i < ranges.Count; i++)
        {
            var r = ranges[i];

            if (r.Start <= current.End + 1)
            {
                current.End = Math.Max(current.End, r.End);
            }
            else
            {
                merged.Add(current);
                current = r;
            }
        }
        merged.Add(current);

        long totalFreshIds = 0;

        foreach (var (start, end) in merged)
        {
            totalFreshIds += (end - start + 1);
        }

        return totalFreshIds;
    }
}