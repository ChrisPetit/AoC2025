namespace Day03;

/// <summary>
/// Computes the sum of the largest possible 12-digit numbers that can be formed
/// from each non-empty input line of digits by removing digits while preserving order.
/// 
/// For each line:
/// - Let `toRemove = line.Length - 12, the number of digits allowed to be dropped.
/// - Traverse digits left to right, maintaining a stack of chosen digits.
/// - While the stack is not empty, `toRemove > 0`, and the top of the stack is less than the current digit, pop the stack and decrement `toRemove`.
/// - Push the current digit onto the stack.
/// - After the traversal, take the first 12 digits from the stack, parse them as a 12-digit number, and add it to the total.
/// </summary>
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