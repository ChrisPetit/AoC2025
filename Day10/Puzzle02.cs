namespace Day10;

public static class Puzzle02
{
    private const double Eps = 1e-9;

    public static long Solve(string[] lines)
    {
        long total = 0;
        foreach (var raw in lines)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                continue;
            }

            var (buttons, targets) = ParseMachine(raw);
            total += SolveMachine(buttons, targets);
        }

        return total;
    }

    private static (List<int[]> Buttons, int[] Targets) ParseMachine(string line)
    {
        var close = line.IndexOf(']');
        if (close < 0)
        {
            throw new ArgumentException("Invalid machine definition.", nameof(line));
        }

        var index = close + 1;
        var buttons = new List<int[]>();
        while (index < line.Length)
        {
            while (index < line.Length && char.IsWhiteSpace(line[index]))
            {
                index++;
            }

            if (index >= line.Length || line[index] == '{')
            {
                break;
            }

            if (line[index] != '(')
            {
                throw new ArgumentException("Invalid button segment.", nameof(line));
            }

            var end = line.IndexOf(')', index);
            if (end < 0)
            {
                throw new ArgumentException("Unterminated button segment.", nameof(line));
            }

            var inner = line.Substring(index + 1, end - index - 1);
            var counters = string.IsNullOrWhiteSpace(inner)
                ? Array.Empty<int>()
                : inner.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => int.Parse(s.Trim()))
                    .ToArray();
            buttons.Add(counters);
            index = end + 1;
        }

        var braceStart = line.IndexOf('{', index);
        var braceEnd = line.IndexOf('}', braceStart >= 0 ? braceStart : index);
        if (braceStart < 0 || braceEnd < 0)
        {
            throw new ArgumentException("Missing counter targets.", nameof(line));
        }

        var targetValues = line.Substring(braceStart + 1, braceEnd - braceStart - 1)
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => int.Parse(s.Trim()))
            .ToArray();

        return (buttons, targetValues);
    }

    private static long SolveMachine(List<int[]> buttons, int[] targets)
    {
        var counters = targets.Length;
        var buttonCount = buttons.Count;
        var rowCount = 2 * counters + buttonCount;
        var colCount = buttonCount + 1;

        var rows = new List<double[]>(rowCount);
        for (var i = 0; i < rowCount; i++)
        {
            rows.Add(new double[colCount]);
        }

        for (var b = 0; b < buttonCount; b++)
        {
            foreach (var counter in buttons[b])
            {
                rows[counter][b] = 1;
                rows[counters + counter][b] = -1;
            }

            rows[2 * counters + b][b] = -1;
        }

        for (var c = 0; c < counters; c++)
        {
            rows[c][colCount - 1] = targets[c];
            rows[counters + c][colCount - 1] = -targets[c];
        }

        var objective = Enumerable.Repeat(1.0, buttonCount).ToArray();
        var best = double.PositiveInfinity;
        var active = new List<double[]>(rows);

        Branch();

        if (double.IsPositiveInfinity(best))
        {
            throw new InvalidOperationException("No feasible solution found.");
        }

        return (long)Math.Round(best);

        void Branch()
        {
            var matrix = active.Select(r => (double[])r.Clone()).ToArray();
            var (value, solution) = Simplex(matrix, objective);
            if (double.IsNegativeInfinity(value) || solution == null || value + Eps >= best)
            {
                return;
            }

            var fractionalIndex = -1;
            double fractionalValue = 0;
            for (var i = 0; i < buttonCount; i++)
            {
                var current = solution[i];
                var rounded = Math.Round(current);
                if (!(Math.Abs(current - rounded) > Eps)) continue;
                fractionalIndex = i;
                fractionalValue = current;
                break;
            }

            if (fractionalIndex == -1)
            {
                best = value;
                return;
            }

            var floor = Math.Floor(fractionalValue);
            var upper = new double[colCount];
            upper[fractionalIndex] = 1;
            upper[colCount - 1] = floor;
            active.Add(upper);
            Branch();
            active.RemoveAt(active.Count - 1);

            var ceil = Math.Ceiling(fractionalValue);
            var lower = new double[colCount];
            lower[fractionalIndex] = -1;
            lower[colCount - 1] = -ceil;
            active.Add(lower);
            Branch();
            active.RemoveAt(active.Count - 1);
        }
    }

    private static (double Value, double[]? Solution) Simplex(double[][] constraints, double[] objective)
    {
        var m = constraints.Length;
        var n = constraints[0].Length - 1;
        var rows = m + 2;
        var cols = n + 2;

        var tableau = new double[rows][];
        for (var i = 0; i < rows; i++)
        {
            tableau[i] = new double[cols];
        }

        for (var i = 0; i < m; i++)
        {
            for (var j = 0; j < n; j++)
            {
                tableau[i][j] = constraints[i][j];
            }

            tableau[i][n] = -1;
            tableau[i][cols - 1] = constraints[i][n];
        }

        for (var j = 0; j < n; j++)
        {
            tableau[m][j] = objective[j];
        }

        tableau[rows - 1][n] = 1;

        var nonBasic = new int[n + 1];
        for (var i = 0; i < n; i++)
        {
            nonBasic[i] = i;
        }

        nonBasic[n] = -1;

        var basic = new int[m];
        for (var i = 0; i < m; i++)
        {
            basic[i] = n + i;
        }

        var rhsIndex = cols - 1;

        var r = 0;
        for (var i = 1; i < m; i++)
        {
            if (tableau[i][rhsIndex] < tableau[r][rhsIndex])
            {
                r = i;
            }
        }

        if (tableau[r][rhsIndex] < -Eps)
        {
            Pivot(r, n);
            if (!Find(1) || tableau[rows - 1][rhsIndex] < -Eps)
            {
                return (double.NegativeInfinity, null);
            }
        }

        for (var i = 0; i < m; i++)
        {
            if (basic[i] != -1) continue;
            var s = 0;
            for (var j = 1; j < n; j++)
            {
                if (Compare(tableau[i][j], nonBasic[j], tableau[i][s], nonBasic[s]) < 0)
                {
                    s = j;
                }
            }

            Pivot(i, s);
        }

        if (!Find(0))
        {
            return (double.NegativeInfinity, null);
        }

        var solution = new double[n];
        for (var i = 0; i < m; i++)
        {
            if (basic[i] >= 0 && basic[i] < n)
            {
                solution[basic[i]] = tableau[i][rhsIndex];
            }
        }

        double value = 0;
        for (var i = 0; i < n; i++)
        {
            value += objective[i] * solution[i];
        }

        return (value, solution);

        bool Find(int phase)
        {
            while (true)
            {
                var s = -1;
                var bestVal = double.PositiveInfinity;
                var bestIdx = int.MaxValue;

                for (var i = 0; i <= n; i++)
                {
                    if (phase == 0 && nonBasic[i] == -1)
                    {
                        continue;
                    }

                    var coefficient = tableau[m + phase][i];
                    if (!(coefficient < bestVal - Eps) &&
                        (!(Math.Abs(coefficient - bestVal) <= Eps) || nonBasic[i] >= bestIdx)) continue;
                    bestVal = coefficient;
                    bestIdx = nonBasic[i];
                    s = i;
                }

                if (bestVal > -Eps)
                {
                    return true;
                }

                var leaving = -1;
                var best = double.PositiveInfinity;
                var bestBasic = int.MaxValue;

                for (var i = 0; i < m; i++)
                {
                    var coeff = tableau[i][s];
                    if (!(coeff > Eps)) continue;
                    var ratio = tableau[i][rhsIndex] / coeff;
                    if (!(ratio < best - Eps) && (!(Math.Abs(ratio - best) <= Eps) || basic[i] >= bestBasic)) continue;
                    best = ratio;
                    bestBasic = basic[i];
                    leaving = i;
                }

                if (leaving == -1)
                {
                    return false;
                }

                Pivot(leaving, s);
            }
        }

        void Pivot(int row, int col)
        {
            var inv = 1.0 / tableau[row][col];

            for (var i = 0; i < rows; i++)
            {
                if (i == row)
                {
                    continue;
                }

                var factor = tableau[i][col] * inv;
                if (Math.Abs(factor) <= Eps)
                {
                    continue;
                }

                for (var j = 0; j < cols; j++)
                {
                    if (j == col)
                    {
                        continue;
                    }

                    tableau[i][j] -= tableau[row][j] * factor;
                }
            }

            for (var j = 0; j < cols; j++)
            {
                tableau[row][j] *= inv;
            }

            for (var i = 0; i < rows; i++)
            {
                if (i == row)
                {
                    continue;
                }

                tableau[i][col] *= -inv;
            }

            tableau[row][col] = inv;
            (basic[row], nonBasic[col]) = (nonBasic[col], basic[row]);
        }

        int Compare(double valA, int idxA, double valB, int idxB)
        {
            if (valA < valB - Eps) return -1;
            if (valA > valB + Eps) return 1;
            if (idxA < idxB) return -1;
            return idxA > idxB ? 1 : 0;
        }
    }
}