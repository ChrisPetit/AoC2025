namespace Day03;

public static class Puzzle02
{
    private const int PickCount = 12;

    public static long Solve(string[] lines)
    {
        long total = 0;

        foreach (var raw in lines)
        {
            var line = raw.Trim();
            if (line.Length == 0)
                continue;

            var n = line.Length;
            if (n < PickCount)
                continue;

            var toRemove = n - PickCount;
            var stack = new char[n];
            var top = 0;

            foreach (var ch in line)
            {
                while (top > 0 && toRemove > 0 && stack[top - 1] < ch)
                {
                    top--;
                    toRemove--;
                }

                stack[top++] = ch;
            }

            long value = 0;
            for (var i = 0; i < PickCount; i++)
            {
                value = value * 10 + (stack[i] - '0');
            }

            total += value;
        }

        return total;
    }
}