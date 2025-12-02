namespace Day02;

public static class Puzzle01
{
    public static long Solve(string[] lines)
    {

        var invalidIds = new List<long>();
        foreach (var line in lines)
        {
            var parts = line.Split("-");
            var start = long.Parse(parts[0]);
            var end = long.Parse(parts[1]);
            var ids = new List<long>();
            for (var id = start; id <= end; id++)
            {
                ids.Add(id);
            }
            
            foreach (var id in ids)
            {
                if (id.ToString().Length % 2 == 1)
                {
                    continue;
                }
                var halfId = id.ToString().Length / 2;
                var firstHalf = id.ToString()[..halfId];
                var secondHalf = id.ToString().Substring(halfId, halfId);
                if (firstHalf == secondHalf)
                {
                    invalidIds.Add(id);
                }
            }
            
        }
        return invalidIds.Sum();
    }
}