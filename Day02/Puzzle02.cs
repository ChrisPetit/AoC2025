namespace Day02;

public static class Puzzle02
{
    public static long Solve(string[] lines)
    {

        var invalidIds = new List<long>();
        foreach (var line in lines)
        {
            var parts = line.Split("-");
            var start = long.Parse(parts[0]);
            var end = long.Parse(parts[1]);

            for (var id = start; id <= end; id++)
            {
                var s = id.ToString();
                var span = s.AsSpan();
                var length = span.Length;
                var invalid = false;

                for (var partLenght = 1; partLenght <= length / 2; partLenght++)
                {
                    if (length % partLenght != 0) continue;
                    
                    var repeats = length / partLenght;
                    if (repeats < 2) continue;
                    
                    var partSpan = span[..partLenght];
                    var allMatch = true;
                    
                    for (var r = 1; r < repeats; r++)
                    {
                        if (partSpan.SequenceEqual(span.Slice(r * partLenght, partLenght))) continue;
                        allMatch = false;
                        break;
                    }

                    if (!allMatch) continue;
                    invalid = true;
                    break;
                }
                if (invalid) 
                {
                    invalidIds.Add(id);
                }
            }
        }
        return invalidIds.Sum();
    }
}