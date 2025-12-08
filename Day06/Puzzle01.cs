namespace Day06;

/// <summary>
/// 
/// </summary>
public static class Puzzle01
{
    public static long Solve(string[]? lines)
    {
        if (lines == null || lines.Length == 0)
            return 0;

        var rows = lines.Length;
        // Determine max width
        var cols = lines.Select(line => line.Length).Prepend(0).Max();
        
        // Build padded grid
        var grid = new char[rows, cols];
        for (var r = 0; r < rows; r++)
        {
            var line = lines[r];
            for (var c = 0; c < cols; c++)
                grid[r, c] = c < line.Length ? line[c] : ' ';
        }

        long grandTotal = 0;
        var col = 0;

        while (col < cols)
        {
            // Skip blank columns
            if (IsBlankColumn(grid, rows, col))
            {
                col++;
                continue;
            }

            // Start of a problem block
            var startCol = col;
            var endCol = col;

            while (endCol + 1 < cols && !IsBlankColumn(grid, rows, endCol + 1))
                endCol++;

            grandTotal += SolveBlock(grid, rows, startCol, endCol);

            col = endCol + 1;
        }

        return grandTotal;
    }

    private static bool IsBlankColumn(char[,] grid, int rows, int col)
    {
        for (var r = 0; r < rows; r++)
        {
            if (grid[r, col] != ' ')
                return false;
        }
        return true;
    }

    private static long SolveBlock(char[,] grid, int rows, int startCol, int endCol)
    {
        // Find operator from bottom to top within this block
        var op = '?';
        for (var r = rows - 1; r >= 0 && op == '?'; r--)
        {
            for (var c = startCol; c <= endCol; c++)
            {
                var ch = grid[r, c];
                if (ch != '+' && ch != '*') continue;
                op = ch;
                break;
            }
        }

        if (op != '+' && op != '*')
            return 0;

        var numbers = new List<long>();

        // For each row, read a number if present
        for (var r = 0; r < rows; r++)
        {
            var span = new char[endCol - startCol + 1];
            var idx = 0;
            for (var c = startCol; c <= endCol; c++)
                span[idx++] = grid[r, c];

            var s = new string(span).Trim();
            switch (s.Length)
            {
                case 0:
                // Skip the operator row itself
                case 1 when (s[0] == '+' || s[0] == '*'):
                    continue;
                default:
                    numbers.Add(long.Parse(s));
                    break;
            }
        }

        if (numbers.Count == 0)
            return 0;

        var result = op == '+' ? 0L : 1L;

        if (op == '+')
        {
            result += numbers.Sum();
        }
        else
        {
            result = numbers.Aggregate(result, 
                (current, n) => current * n);
        }

        return result;
    }
}