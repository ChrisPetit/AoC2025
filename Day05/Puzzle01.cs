namespace Day05;

public static class Puzzle01
{
    public static long Solve(string[] lines)
    {
        var ranges = new List<(long Start, long End)>();
        var ids = new List<long>();

        var inRanges = true;

        foreach (var raw in lines)
        {
            var line = raw.Trim();
            if (line.Length == 0)
            {
                inRanges = false;
                continue;
            }

            if (inRanges)
            {
                var parts = line.Split('-');
                if (parts.Length != 2)
                    continue;

                var start = long.Parse(parts[0]);
                var end = long.Parse(parts[1]);
                if (start > end)
                    (start, end) = (end, start);

                ranges.Add((start, end));
            }
            else
            {
                ids.Add(long.Parse(line));
            }
        }

        if (ranges.Count == 0 || ids.Count == 0)
            return 0;

        ranges.Sort((x, y) => x.Start.CompareTo(y.Start));

        var merged = new List<(long Start, long End)>();
        var cur = ranges[0];

        for (var i = 1; i < ranges.Count; i++)
        {
            var r = ranges[i];
            if (r.Start <= cur.End + 1)
            {
                cur.End = Math.Max(cur.End, r.End);
            }
            else
            {
                merged.Add(cur);
                cur = r;
            }
        }

        merged.Add(cur);

        return ids.LongCount(id => IsFresh(id, merged));
    }

    private static bool IsFresh(long id, List<(long Start, long End)> merged)
    {
        var lo = 0;
        var hi = merged.Count - 1;

        while (lo <= hi)
        {
            var mid = (lo + hi) / 2;
            var (start, end) = merged[mid];

            if (id < start)
            {
                hi = mid - 1;
            }
            else if (id > end)
            {
                lo = mid + 1;
            }
            else
            {
                return true;
            }
        }

        return false;
    }
}