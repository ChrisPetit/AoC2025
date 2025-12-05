namespace Day03;

/// <summary>
/// 1. Track the best pair (best) as an int, initialized to -1.
/// 2. For every index i, and every later index j > i:
///     - Form pair = (line[i] - '0') * 10 + (line[j] - '0').
///     - Update best = Math.Max(best, pair).
/// 3. Add best to the running total.
/// </summary>
public static class Puzzle01
{
    public static int Solve(string[] lines)
    {
        var total = 0;

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var span = line.AsSpan();
            var best = -1;

            for (var i = 0; i < span.Length - 1; i++)
            {
                var d1 = span[i] - '0';
                for (var j = i + 1; j < span.Length; j++)
                {
                    var d2 = span[j] - '0';
                    var value = d1 * 10 + d2;
                    if (value > best)
                        best = value;
                }
            }

            if (best >= 0)
                total += best;
        }

        return total;
    }
}