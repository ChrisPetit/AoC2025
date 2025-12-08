namespace Day08;

/// <summary>
/// Computes the product of the X-coordinates of the two junction boxes whose
/// connection causes all boxes to become part of a single connected component,
/// when edges between boxes are added in order of increasing squared distance.
/// </summary>
public static class Puzzle02
{
    public static long Solve(string[]? lines)
    {
        if (lines == null)
            return 0;

        var points = lines
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(line =>
            {
                var parts = line.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                return parts.Length != 3
                    ? throw new FormatException("Invalid junction box coordinates.")
                    : (X: long.Parse(parts[0]), Y: long.Parse(parts[1]), Z: long.Parse(parts[2]));
            })
            .ToList();

        if (points.Count < 2)
            return 0;

        var edges = new List<(int A, int B, long DistSq)>();
        for (var i = 0; i < points.Count; i++)
        {
            for (var j = i + 1; j < points.Count; j++)
            {
                var (x1, y1, z1) = points[i];
                var (x2, y2, z2) = points[j];
                var dx = x1 - x2;
                var dy = y1 - y2;
                var dz = z1 - z2;
                edges.Add((i, j, dx * dx + dy * dy + dz * dz));
            }
        }

        edges.Sort((lhs, rhs) => lhs.DistSq.CompareTo(rhs.DistSq));

        var n = points.Count;
        var parent = new int[n];
        var size = new int[n];
        for (var i = 0; i < n; i++)
        {
            parent[i] = i;
            size[i] = 1;
        }

        var components = n;

        foreach (var (a, b, _) in edges)
        {
            if (!Union(a, b))
                continue;

            if (components != 1) continue;
            var p1 = points[a];
            var p2 = points[b];
            return p1.X * p2.X;
        }

        return 0;

        bool Union(int a, int b)
        {
            var rootA = Find(a);
            var rootB = Find(b);
            if (rootA == rootB)
                return false;

            if (size[rootA] < size[rootB])
                (rootA, rootB) = (rootB, rootA);

            parent[rootB] = rootA;
            size[rootA] += size[rootB];
            components--;
            return true;
        }

        int Find(int node)
        {
            while (parent[node] != node)
            {
                parent[node] = parent[parent[node]];
                node = parent[node];
            }

            return node;
        }
    }
}