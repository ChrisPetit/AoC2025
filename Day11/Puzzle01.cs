namespace Day11;

public static class Puzzle01
{
    public static long Solve(string[] lines)
    {
        var graph = new Dictionary<string, List<string>>(StringComparer.Ordinal);

        foreach (var raw in lines)
        {
            if (string.IsNullOrWhiteSpace(raw))
                continue;

            var parts = raw.Split(':', 2, StringSplitOptions.TrimEntries);
            var node = parts[0];
            var targets = parts.Length > 1
                ? parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                : Array.Empty<string>();

            if (!graph.TryGetValue(node, out var list))
                graph[node] = list = new List<string>();

            list.AddRange(targets);
        }

        var memo = new Dictionary<string, long>(StringComparer.Ordinal);

        long Count(string node)
        {
            if (node == "out")
                return 1;

            if (memo.TryGetValue(node, out var cached))
                return cached;

            if (!graph.TryGetValue(node, out var edges))
                return memo[node] = 0;

            long total = 0;
            foreach (var next in edges)
                total += Count(next);

            return memo[node] = total;
        }

        return Count("you");
    }
}