# Advent of Code 2025 Solutions in C# and .NET 10

This solution is an implementation of the Advent of Code 2025 puzzles in C#, organized by day and puzzle part.
## Project Structure
Each **DayXX** folder contains:
- **Puzzle01.cs**: solution for part 1 of that day's puzzle.
- **Puzzle02.cs**: solution for part 2 of that day's puzzle.
- A small Program entry point that reads the input file for the corresponding day, calls the appropriate Solve method, and writes the answer to the console.

## Implementation Details
The solutions are implemented using C# 12 and .NET 10, leveraging modern language features and libraries for clarity and performance.

The project focuses on:
- Clear, readable solutions that directly express the puzzle logic.
- Efficient handling of typical Advent of Code input sizes (e.g. iterating over ranges, using spans to avoid unnecessary allocations).
- Keeping each puzzle's logic self-contained so it is easy to run or refactor independently.

To run a specific day's solution, open the project in **JetBrains Rider** (or your prefered IDE), set the appropriate startup project (or adjust the **Program** in the corresponding **DayXX**), and run. Input files should be placed alongside the day's code (or in whatever location the **Program** for that day expects) and passed into the **Solve** method as lines of text.