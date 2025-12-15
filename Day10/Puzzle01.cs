namespace Day10;

public static partial class Puzzle01
{
    public static long Solve(string[] lines)
    {
        return lines.Aggregate<string?, long>(0, (current, line) => current + SolveMachine(line));
    }

    private static int SolveMachine(string? line)
    {
        // Parse: [target] (btn1) (btn2) ... {ignore}
        var targetMatch = MyRegex().Match(line!);
        var target = targetMatch.Groups[1].Value;
        var numLights = target.Length;

        var buttonMatches = System.Text.RegularExpressions.Regex.Matches(line!, @"\(([^)]*)\)");
        var buttons = new List<int[]>();

        foreach (System.Text.RegularExpressions.Match match in buttonMatches)
        {
            if (match.Index >= line!.IndexOf('{')) continue;
            var indices = match.Groups[1].Value.Split(',')
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(int.Parse)
                .ToArray();
            buttons.Add(indices);
        }

        var numButtons = buttons.Count;
        var matrix = new bool[numLights, numButtons + 1];

        for (var light = 0; light < numLights; light++)
        {
            for (var btn = 0; btn < numButtons; btn++)
            {
                if (buttons[btn].Contains(light))
                {
                    matrix[light, btn] = true;
                }
            }

            matrix[light, numButtons] = target[light] == '#';
        }

        return FindMinimumPresses(matrix, numButtons);
    }

    private static int FindMinimumPresses(bool[,] matrix, int numButtons)
    {
        var numLights = matrix.GetLength(0);
        var minPresses = int.MaxValue;

        // Try all possible combinations of button presses
        for (var mask = 0; mask < (1 << numButtons); mask++)
        {
            var presses = new bool[numButtons];
            for (var i = 0; i < numButtons; i++)
            {
                presses[i] = (mask & (1 << i)) != 0;
            }

            // Check if this combination achieves the target
            var valid = true;
            for (var light = 0; light < numLights; light++)
            {
                var state = false;
                for (var btn = 0; btn < numButtons; btn++)
                {
                    if (presses[btn] && matrix[light, btn])
                    {
                        state = !state;
                    }
                }

                if (state == matrix[light, numButtons]) continue;
                valid = false;
                break;
            }

            if (valid)
            {
                minPresses = Math.Min(minPresses, presses.Count(x => x));
            }
        }

        return minPresses == int.MaxValue ? 0 : minPresses;
    }
    
    [System.Text.RegularExpressions.GeneratedRegex(@"\[([.#]+)\]")]
    private static partial System.Text.RegularExpressions.Regex MyRegex();
}