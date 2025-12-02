using Day02;

var input = File.ReadAllLines("input");
var lines = input[0].Split(',');

Console.WriteLine(Puzzle01.Solve(lines));
Console.WriteLine(Puzzle02.Solve(lines));