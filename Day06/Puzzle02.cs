namespace Day06;

/// <summary>
/// Solves the second puzzle by scanning the input as a 2D grid, splitting it
/// into vertical blocks separated by blank columns, and for each block:
/// * determines the operator (`+` or `*`) from the bottom row,
/// * reads each column as a number (top to bottom),
/// * evaluates the block expression, and
/// returns the sum of all block results.
/// </summary>
public static class Puzzle02
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
        // Operator is on the bottom row within the block
        var op = '?';
        for (var c = startCol; c <= endCol; c++)
        {
            var ch = grid[rows - 1, c];
            if (ch != '+' && ch != '*') continue;
            op = ch;
            break;
        }

        if (op != '+' && op != '*')
            return 0;

        var numbers = new List<long>();

        // Each column encodes one number (top to bottom)
        for (var c = startCol; c <= endCol; c++)
        {
            var sb = new System.Text.StringBuilder();
            for (var r = 0; r < rows; r++)
            {
                var ch = grid[r, c];
                if (ch == ' ' || ch == '+' || ch == '*')
                    continue;
                sb.Append(ch);
            }

            if (sb.Length == 0)
                continue;

            numbers.Add(long.Parse(sb.ToString()));
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