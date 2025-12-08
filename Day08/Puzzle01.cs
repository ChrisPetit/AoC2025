namespace Day08;

/// <summary>
/// Computes the product of the sizes of the three largest clusters formed by
/// connecting nearby 3D points. Each input line is parsed into a 3D point,
/// all pairwise squared distances are computed and sorted, and then only the
/// closest `connectionsToAttempt` pairs are united using a Disjoint Set
/// Union structure. Finally, the sizes of all connected components are
/// collected, the three largest are selected, and their sizes are multiplied
/// together. If there are fewer than three components, the method returns 0.
/// </summary>
public static class Puzzle01
{
    private readonly struct Point3D
    {
        private readonly long _x;
        private readonly long _y;
        private readonly long _z;

        private Point3D(long x, long y, long z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public static Point3D Parse(string line)
        {
            var parts = line.Split(',').Select(long.Parse).ToArray();
            return new Point3D(parts[0], parts[1], parts[2]);
        }

        public static long SquaredDistance(Point3D a, Point3D b)
        {
            var dx = a._x - b._x;
            var dy = a._y - b._y;
            var dz = a._z - b._z;
            return dx * dx + dy * dy + dz * dz;
        }
    }

    private sealed class DisjointSetUnion
    {
        private readonly int[] _parent;
        private readonly int[] _size;

        public DisjointSetUnion(int n)
        {
            _parent = new int[n];
            _size = new int[n];
            for (var i = 0; i < n; i++)
            {
                _parent[i] = i;
                _size[i] = 1;
            }
        }

        private int Find(int x)
        {
            if (_parent[x] == x)
                return x;
            return _parent[x] = Find(_parent[x]);
        }

        public void Union(int a, int b)
        {
            var ra = Find(a);
            var rb = Find(b);
            if (ra == rb)
                return;

            if (_size[ra] < _size[rb])
            {
                _parent[ra] = rb;
                _size[rb] += _size[ra];
            }
            else
            {
                _parent[rb] = ra;
                _size[ra] += _size[rb];
            }
        }

        public IEnumerable<int> ComponentSizes()
        {
            var counts = new Dictionary<int, int>();
            for (var i = 0; i < _parent.Length; i++)
            {
                var root = Find(i);
                counts[root] = counts.TryGetValue(root, out var c) ? c + 1 : 1;
            }

            return counts.Values;
        }
    }

    public static long Solve(string[]? lines)
    {
        if (lines == null || lines.Length == 0)
            return 0;

        var points = lines.Select(Point3D.Parse).ToArray();
        var connections = new List<(int A, int B, long Dist)>();

        for (var i = 0; i < points.Length; i++)
        {
            for (var j = i + 1; j < points.Length; j++)
            {
                connections.Add((i, j, Point3D.SquaredDistance(points[i], points[j])));
            }
        }

        connections.Sort((x, y) => x.Dist.CompareTo(y.Dist));

        var connectionsToAttempt = points.Length <= 20 ? 10 : 1000;
        var dsu = new DisjointSetUnion(points.Length);

        var attempts = 0;
        foreach (var (a, b, _) in connections)
        {
            if (attempts >= connectionsToAttempt)
                break;

            dsu.Union(a, b);
            attempts++;
        }

        var topThree = dsu.ComponentSizes()
            .OrderByDescending(s => s)
            .Take(3)
            .ToArray();

        return topThree.Length < 3 ? 0 : (long)topThree[0] * topThree[1] * topThree[2];
    }
}