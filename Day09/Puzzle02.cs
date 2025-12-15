namespace Day09;

/// <summary>
/// Finds the largest axis-aligned rectangle whose corners are red points and whose interior
/// does not intersect the exterior of the red polygon.
///
/// The method:
/// - Parses the input lines into a cyclic list of red points forming a rectilinear polygon.
/// - Compresses the X and Y coordinates into bounds and span arrays to work on a compact grid.
/// - Marks all grid cells covered by the red polygon edges as blocked.
/// - Flood-fills from the grid border to find \`outside\` cells (reachable from "infinity").
/// - Builds an \`allowed\` grid of cells that are either blocked (on the boundary) or not outside
///   (i.e. interior or boundary of the polygon).
/// - Builds a 2D prefix-sum of allowed area, weighted by the real coordinate spans.
/// - Enumerates every pair of red points as opposite corners of an axis-aligned rectangle and,
///   via the prefix-sum, checks if the entire rectangle area is allowed.
/// - Returns the maximum area of such a rectangle, or 0 if none exists.
/// </summary>
public static class Puzzle02
{
    private readonly record struct Point(long X, long Y);

    public static long Solve(string[]? lines)
    {
        if (lines == null)
            return 0;

        var red = ParseRed(lines);
        if (red.Count < 2)
            return 0;

        var xBounds = BuildAxisBounds(red, static p => p.X, 2);
        var yBounds = BuildAxisBounds(red, static p => p.Y, 2);

        var xIndex = BuildIndex(xBounds);
        var yIndex = BuildIndex(yBounds);

        var xSpans = BuildSpans(xBounds);
        var ySpans = BuildSpans(yBounds);

        var blocked = BuildBlockedGrid(red, xIndex, yIndex, yBounds.Count - 1, xBounds.Count - 1);
        var outside = FloodFillOutside(blocked);
        var allowed = BuildAllowed(blocked, outside);
        var prefix = BuildPrefix(allowed, xSpans, ySpans);

        long best = 0;
        for (var i = 0; i < red.Count - 1; i++)
        {
            for (var j = i + 1; j < red.Count; j++)
            {
                var a = red[i];
                var b = red[j];

                var leftCoord = Math.Min(a.X, b.X);
                var rightCoord = Math.Max(a.X, b.X) + 1;
                var bottomCoord = Math.Min(a.Y, b.Y);
                var topCoord = Math.Max(a.Y, b.Y) + 1;

                var leftIdx = xIndex[leftCoord];
                var rightIdx = xIndex[rightCoord];
                var bottomIdx = yIndex[bottomCoord];
                var topIdx = yIndex[topCoord];

                var area = (rightCoord - leftCoord) * (topCoord - bottomCoord);
                var allowedArea = Query(prefix, leftIdx, bottomIdx, rightIdx, topIdx);

                if (allowedArea == area && area > best)
                    best = area;
            }
        }

        return best;
    }

    private static List<Point> ParseRed(string[] lines)
    {
        var result = new List<Point>();
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var parts = line.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
                throw new FormatException($"Invalid coordinate: {line}");

            result.Add(new Point(
                long.Parse(parts[0]),
                long.Parse(parts[1])));
        }

        return result;
    }

    private static List<long> BuildAxisBounds(IReadOnlyList<Point> points, Func<Point, long> selector, long padding)
    {
        var bounds = new SortedSet<long>();
        var min = long.MaxValue;
        var max = long.MinValue;

        foreach (var point in points)
        {
            var value = selector(point);
            bounds.Add(value);
            bounds.Add(SafeOffset(value, 1));
            min = Math.Min(min, value);
            max = Math.Max(max, value);
        }

        bounds.Add(SafeOffset(min, -padding));
        bounds.Add(SafeOffset(max, padding + 1));

        return [..bounds];
    }

    private static Dictionary<long, int> BuildIndex(IReadOnlyList<long> bounds)
    {
        var index = new Dictionary<long, int>(bounds.Count);
        for (var i = 0; i < bounds.Count; i++)
            index[bounds[i]] = i;
        return index;
    }

    private static long[] BuildSpans(IReadOnlyList<long> bounds)
    {
        var spans = new long[bounds.Count - 1];
        for (var i = 0; i < spans.Length; i++)
            spans[i] = bounds[i + 1] - bounds[i];
        return spans;
    }

    private static bool[,] BuildBlockedGrid(IReadOnlyList<Point> red, IReadOnlyDictionary<long, int> xIndex,
        IReadOnlyDictionary<long, int> yIndex, int rows, int cols)
    {
        var blocked = new bool[rows, cols];

        for (var i = 0; i < red.Count; i++)
        {
            var (x, y) = red[i];
            var end = red[(i + 1) % red.Count];

            if (x == end.X && y == end.Y)
                continue;

            if (x != end.X && y != end.Y)
                throw new InvalidOperationException("Adjacent red tiles must align horizontally or vertically.");

            if (x == end.X)
            {
                var colStart = xIndex[x];
                var colEnd = xIndex[SafeOffset(x, 1)];
                var minY = Math.Min(y, end.Y);
                var maxY = Math.Max(y, end.Y);
                var rowStart = yIndex[minY];
                var rowEnd = yIndex[SafeOffset(maxY, 1)];
                for (var row = rowStart; row < rowEnd; row++)
                {
                    for (var col = colStart; col < colEnd; col++)
                        blocked[row, col] = true;
                }
            }
            else
            {
                var rowStart = yIndex[y];
                var rowEnd = yIndex[SafeOffset(y, 1)];
                var minX = Math.Min(x, end.X);
                var maxX = Math.Max(x, end.X);
                var colStart = xIndex[minX];
                var colEnd = xIndex[SafeOffset(maxX, 1)];
                for (var row = rowStart; row < rowEnd; row++)
                {
                    for (var col = colStart; col < colEnd; col++)
                        blocked[row, col] = true;
                }
            }
        }

        return blocked;
    }

    private static bool[,] FloodFillOutside(bool[,] blocked)
    {
        var rows = blocked.GetLength(0);
        var cols = blocked.GetLength(1);
        var visited = new bool[rows, cols];
        var queue = new Queue<(int Row, int Col)>();

        for (var col = 0; col < cols; col++)
        {
            TryEnqueue(0, col);
            TryEnqueue(rows - 1, col);
        }

        for (var row = 0; row < rows; row++)
        {
            TryEnqueue(row, 0);
            TryEnqueue(row, cols - 1);
        }

        var directions = new (int Row, int Col)[] { (-1, 0), (1, 0), (0, -1), (0, 1) };
        while (queue.Count > 0)
        {
            var (row, col) = queue.Dequeue();
            foreach (var (dr, dc) in directions)
                TryEnqueue(row + dr, col + dc);
        }

        return visited;

        void TryEnqueue(int row, int col)
        {
            if (row < 0 || col < 0 || row >= rows || col >= cols)
                return;
            if (blocked[row, col] || visited[row, col])
                return;
            visited[row, col] = true;
            queue.Enqueue((row, col));
        }
    }

    private static bool[,] BuildAllowed(bool[,] blocked, bool[,] outside)
    {
        var rows = blocked.GetLength(0);
        var cols = blocked.GetLength(1);
        var allowed = new bool[rows, cols];

        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
                allowed[row, col] = blocked[row, col] || !outside[row, col];
        }

        return allowed;
    }

    private static long[,] BuildPrefix(bool[,] allowed, long[] xSpans, long[] ySpans)
    {
        var rows = allowed.GetLength(0);
        var cols = allowed.GetLength(1);
        var prefix = new long[rows + 1, cols + 1];

        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                var val = allowed[row, col] ? xSpans[col] * ySpans[row] : 0;
                prefix[row + 1, col + 1] = val + prefix[row, col + 1] + prefix[row + 1, col] - prefix[row, col];
            }
        }

        return prefix;
    }

    private static long Query(long[,] prefix, int left, int bottom, int right, int top)
    {
        return prefix[top, right] - prefix[bottom, right] - prefix[top, left] + prefix[bottom, left];
    }

    private static long SafeOffset(long value, long delta)
    {
        return delta switch
        {
            > 0 when value > long.MaxValue - delta => long.MaxValue,
            < 0 when value < long.MinValue - delta => long.MinValue,
            _ => value + delta
        };
    }
}