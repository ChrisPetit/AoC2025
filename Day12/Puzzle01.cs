namespace Day12;

public static class Puzzle01
{
    public static long Solve(string[] lines)
    {
        var normalized = NormalizeLines(lines);
        var (shapes, regionStart) = ParseShapes(normalized);
        var regions = ParseRegions(normalized, regionStart, shapes.Count);

        return regions.LongCount(region => CanFitAll(region, shapes));
    }

    private static string[] NormalizeLines(string[]? lines)
    {
        if (lines == null)
            return Array.Empty<string>();

        var result = new string[lines.Length];
        for (var i = 0; i < lines.Length; i++)
            result[i] = (lines[i]).TrimEnd('\r');
        return result;
    }

    private static (List<ShapeDefinition> Shapes, int NextIndex) ParseShapes(string[] lines)
    {
        var shapes = new List<ShapeDefinition>();
        var index = 0;

        while (index < lines.Length)
        {
            var line = lines[index];
            if (string.IsNullOrWhiteSpace(line))
            {
                index++;
                continue;
            }

            if (IsRegionHeader(line))
                break;

            if (!IsShapeHeader(line))
                throw new InvalidOperationException($"Unexpected line `{line}` while parsing shapes.");

            var colon = line.IndexOf(':');
            var shapeId = int.Parse(line[..colon].Trim(), System.Globalization.CultureInfo.InvariantCulture);

            index++;
            var patternLines = new List<string>();
            while (index < lines.Length)
            {
                var candidate = lines[index];
                if (string.IsNullOrWhiteSpace(candidate))
                {
                    index++;
                    break;
                }

                if (IsShapeHeader(candidate) || IsRegionHeader(candidate))
                    break;

                patternLines.Add(candidate.Trim());
                index++;
            }

            if (patternLines.Count == 0)
                throw new InvalidOperationException($"Shape {shapeId} has no rows.");

            shapes.Add(BuildShape(shapeId, patternLines));
        }

        var ordered = shapes.OrderBy(s => s.Index).ToList();
        for (var i = 0; i < ordered.Count; i++)
        {
            if (ordered[i].Index != i)
                throw new InvalidOperationException("Shape indexes must be contiguous starting at 0.");
        }

        return (ordered, index);
    }

    private static List<RegionRequest> ParseRegions(string[] lines, int startIndex, int shapeCount)
    {
        var regions = new List<RegionRequest>();
        for (var i = startIndex; i < lines.Length; i++)
        {
            var line = lines[i];
            if (string.IsNullOrWhiteSpace(line))
                continue;

            if (!IsRegionHeader(line))
                throw new InvalidOperationException($"Unexpected line `{line}` while parsing regions.");

            var parts = line.Split(':', 2, StringSplitOptions.TrimEntries);
            var sizePart = parts[0].Trim();
            var dimensionTokens =
                sizePart.Split('x', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (dimensionTokens.Length != 2)
                throw new InvalidOperationException($"Invalid region size `{sizePart}`.");

            var width = int.Parse(dimensionTokens[0], System.Globalization.CultureInfo.InvariantCulture);
            var height = int.Parse(dimensionTokens[1], System.Globalization.CultureInfo.InvariantCulture);
            if (width <= 0 || height <= 0)
                throw new InvalidOperationException("Region dimensions must be positive.");

            var counts = parts.Length > 1
                ? parts[1].Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                : Array.Empty<string>();

            if (counts.Length != shapeCount)
                throw new InvalidOperationException("Region counts must match the number of shapes.");

            var demand = new int[shapeCount];
            for (var j = 0; j < counts.Length; j++)
            {
                demand[j] = int.Parse(counts[j], System.Globalization.CultureInfo.InvariantCulture);
                if (demand[j] < 0)
                    throw new InvalidOperationException("Shape counts cannot be negative.");
            }

            regions.Add(new RegionRequest(width, height, demand));
        }

        return regions;
    }

    private static bool CanFitAll(RegionRequest region, IReadOnlyList<ShapeDefinition> shapes)
    {
        var counts = (int[])region.Counts.Clone();
        var totalPieces = counts.Sum();
        if (totalPieces == 0)
            return true;

        var regionCells = region.Width * region.Height;
        var shapeAreas = shapes.Select(s => s.Area).ToArray();
        var remainingArea = counts.Select((t, i) => t * shapeAreas[i]).Sum();

        if (remainingArea > regionCells)
            return false;

        var placementsByShape = BuildPlacements(shapes, region);
        if (counts.Where((t, i) => t > 0 && placementsByShape[i].Length == 0).Any())
        {
            return false;
        }

        var board = new bool[regionCells];
        var occupiedCells = 0;

        return Search(0, remainingArea);

        bool Search(int placedPieces, int areaNeeded)
        {
            if (placedPieces == totalPieces)
                return true;

            if (areaNeeded > regionCells - occupiedCells)
                return false;

            var chosenShape = -1;
            var bestCount = int.MaxValue;

            for (var i = 0; i < counts.Length; i++)
            {
                if (counts[i] == 0)
                    continue;

                var feasible = 0;
                foreach (var placement in placementsByShape[i])
                {
                    if (CanPlace(placement, board))
                    {
                        feasible++;
                        if (feasible >= bestCount)
                            break;
                    }
                }

                if (feasible == 0)
                    return false;

                if (feasible >= bestCount) continue;
                bestCount = feasible;
                chosenShape = i;
            }

            if (chosenShape == -1)
                return false;

            foreach (var placement in placementsByShape[chosenShape])
            {
                if (!CanPlace(placement, board))
                    continue;

                ApplyPlacement(placement, board);
                counts[chosenShape]--;
                occupiedCells += placement.Cells.Length;

                if (Search(placedPieces + 1, areaNeeded - placement.Cells.Length))
                    return true;

                occupiedCells -= placement.Cells.Length;
                counts[chosenShape]++;
                RemovePlacement(placement, board);
            }

            return false;
        }
    }

    private static Placement[][] BuildPlacements(IReadOnlyList<ShapeDefinition> shapes, RegionRequest region)
    {
        var perShape = new List<Placement>[shapes.Count];
        for (var i = 0; i < perShape.Length; i++)
            perShape[i] = [];

        for (var shapeIndex = 0; shapeIndex < shapes.Count; shapeIndex++)
        {
            var shape = shapes[shapeIndex];
            foreach (var orientation in shape.Orientations)
            {
                if (orientation.Width > region.Width || orientation.Height > region.Height)
                    continue;

                for (var offsetY = 0; offsetY <= region.Height - orientation.Height; offsetY++)
                {
                    for (var offsetX = 0; offsetX <= region.Width - orientation.Width; offsetX++)
                    {
                        var cells = new int[orientation.Cells.Length];
                        for (var c = 0; c < orientation.Cells.Length; c++)
                        {
                            var local = orientation.Cells[c];
                            var absoluteX = offsetX + local.X;
                            var absoluteY = offsetY + local.Y;
                            cells[c] = absoluteY * region.Width + absoluteX;
                        }

                        Array.Sort(cells);
                        perShape[shapeIndex].Add(new Placement(cells));
                    }
                }
            }
        }

        var result = new Placement[perShape.Length][];
        for (var i = 0; i < perShape.Length; i++)
            result[i] = perShape[i].ToArray();
        return result;
    }

    private static bool CanPlace(Placement placement, bool[] board)
    {
        return placement.Cells.All(cell => !board[cell]);
    }

    private static void ApplyPlacement(Placement placement, bool[] board)
    {
        foreach (var cell in placement.Cells)
            board[cell] = true;
    }

    private static void RemovePlacement(Placement placement, bool[] board)
    {
        foreach (var cell in placement.Cells)
            board[cell] = false;
    }

    private static ShapeDefinition BuildShape(int index, IReadOnlyList<string> patternLines)
    {
        var grid = BuildGrid(patternLines);
        var orientations = BuildOrientations(grid);
        return new ShapeDefinition(index, orientations);
    }

    private static bool[,] BuildGrid(IReadOnlyList<string> rows)
    {
        if (rows.Count == 0)
            throw new InvalidOperationException("Shape cannot be empty.");

        var width = rows[0].Length;
        if (rows.Any(row => row.Length != width))
            throw new InvalidOperationException("Shape rows must all have the same length.");

        var grid = new bool[rows.Count, width];
        var hasFilledCell = false;

        for (var y = 0; y < rows.Count; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var ch = rows[y][x];
                if (ch == '#')
                {
                    grid[y, x] = true;
                    hasFilledCell = true;
                }
                else if (ch != '.')
                {
                    throw new InvalidOperationException($"Unexpected character `{ch}` in shape definition.");
                }
            }
        }

        return !hasFilledCell ? throw new InvalidOperationException("Shapes must contain at least one `#`.") : Trim(grid);
    }

    private static ShapeOrientation[] BuildOrientations(bool[,] baseGrid)
    {
        var orientations = new List<ShapeOrientation>();
        var seen = new HashSet<string>();

        var current = baseGrid;
        for (var i = 0; i < 4; i++)
        {
            Add(current);
            current = Rotate90(current);
        }

        current = FlipHorizontal(baseGrid);
        for (var i = 0; i < 4; i++)
        {
            Add(current);
            current = Rotate90(current);
        }

        return orientations.ToArray();

        void Add(bool[,] grid)
        {
            var trimmed = Trim(grid);
            var points = ExtractPoints(trimmed);
            var key = BuildOrientationKey(points);
            if (seen.Add(key))
                orientations.Add(new ShapeOrientation(points));
        }
    }

    private static bool[,] Rotate90(bool[,] grid)
    {
        var rows = grid.GetLength(0);
        var cols = grid.GetLength(1);
        var rotated = new bool[cols, rows];

        for (var y = 0; y < rows; y++)
        {
            for (var x = 0; x < cols; x++)
                rotated[x, rows - 1 - y] = grid[y, x];
        }

        return rotated;
    }

    private static bool[,] FlipHorizontal(bool[,] grid)
    {
        var rows = grid.GetLength(0);
        var cols = grid.GetLength(1);
        var flipped = new bool[rows, cols];

        for (var y = 0; y < rows; y++)
        {
            for (var x = 0; x < cols; x++)
                flipped[y, cols - 1 - x] = grid[y, x];
        }

        return flipped;
    }

    private static bool[,] Trim(bool[,] grid)
    {
        var rows = grid.GetLength(0);
        var cols = grid.GetLength(1);
        var minRow = rows;
        var maxRow = -1;
        var minCol = cols;
        var maxCol = -1;

        for (var y = 0; y < rows; y++)
        {
            for (var x = 0; x < cols; x++)
            {
                if (!grid[y, x])
                    continue;

                minRow = Math.Min(minRow, y);
                maxRow = Math.Max(maxRow, y);
                minCol = Math.Min(minCol, x);
                maxCol = Math.Max(maxCol, x);
            }
        }

        if (maxRow == -1)
            throw new InvalidOperationException("Shape contains no filled cells.");

        var trimmed = new bool[maxRow - minRow + 1, maxCol - minCol + 1];
        for (var y = minRow; y <= maxRow; y++)
        {
            for (var x = minCol; x <= maxCol; x++)
                trimmed[y - minRow, x - minCol] = grid[y, x];
        }

        return trimmed;
    }

    private static Point[] ExtractPoints(bool[,] grid)
    {
        var rows = grid.GetLength(0);
        var cols = grid.GetLength(1);
        var points = new List<Point>();

        for (var y = 0; y < rows; y++)
        {
            for (var x = 0; x < cols; x++)
            {
                if (grid[y, x])
                    points.Add(new Point(x, y));
            }
        }

        return points.ToArray();
    }

    private static string BuildOrientationKey(Point[] points) =>
        string.Join(';', points.OrderBy(p => p.Y).ThenBy(p => p.X).Select(p => $"{p.X},{p.Y}"));

    private static bool IsShapeHeader(string line)
    {
        var colon = line.IndexOf(':');
        return colon >= 0 && int.TryParse(line[..colon].Trim(), out _);
    }

    private static bool IsRegionHeader(string line)
    {
        var colon = line.IndexOf(':');
        return colon >= 0 && line[..colon].Contains('x', StringComparison.OrdinalIgnoreCase);
    }

    private sealed class ShapeDefinition(int index, ShapeOrientation[] orientations)
    {
        public int Index { get; } = index;
        public ShapeOrientation[] Orientations { get; } = orientations;
        public int Area { get; } = orientations[0].Cells.Length;
    }

    private sealed class ShapeOrientation
    {
        public ShapeOrientation(Point[] cells)
        {
            Cells = cells;
            Width = cells.Max(static p => p.X) + 1;
            Height = cells.Max(static p => p.Y) + 1;
        }

        public Point[] Cells { get; }
        public int Width { get; }
        public int Height { get; }
    }

    private sealed record RegionRequest(int Width, int Height, int[] Counts);

    private sealed class Placement(int[] cells)
    {
        public int[] Cells { get; } = cells;
    }

    private readonly record struct Point(int X, int Y);
}