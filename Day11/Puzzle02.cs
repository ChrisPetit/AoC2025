namespace Day11;

public static class Puzzle02
{
    private const int FirstBit = 1;
    private const int SecondBit = 2;
    private const int RequiredMask = FirstBit | SecondBit;

    public static long Solve(string[] lines)
    {
        var graph = ParseGraph(lines);
        return CountPathsThroughBoth(graph, "svr", "dac", "fft");
    }

    private static Dictionary<string, List<string>> ParseGraph(IEnumerable<string> lines)
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
            {
                list = new List<string>();
                graph[node] = list;
            }

            list.AddRange(targets);
        }

        return graph;
    }

    private static long CountPathsThroughBoth(
        IReadOnlyDictionary<string, List<string>> graph,
        string start,
        string firstRequired,
        string secondRequired)
    {
        var memo = new Dictionary<(string node, int mask), long>();
        return Dfs(start, 0);

        long Dfs(string node, int mask)
        {
            var nextMask = mask;
            if (string.Equals(node, firstRequired, StringComparison.Ordinal))
                nextMask |= FirstBit;
            if (string.Equals(node, secondRequired, StringComparison.Ordinal))
                nextMask |= SecondBit;

            if (node == "out")
                return nextMask == RequiredMask ? 1 : 0;

            if (!graph.TryGetValue(node, out var edges) || edges.Count == 0)
                return 0;

            if (memo.TryGetValue((node, nextMask), out var cached))
                return cached;

            long total = 0;
            foreach (var child in edges)
                total += Dfs(child, nextMask);

            memo[(node, nextMask)] = total;
            return total;
        }
    }
}