using Day12;
            using Xunit;
            
            namespace Day12.Tests;
            
            public class Puzzle01Tests
            {
                [Fact]
                public void Solve_ReturnsExpectedCount_ForSampleInput()
                {
                    var input = new[]
                    {
                        "0:",
                        "###",
                        "##.",
                        "##.",
                        "",
                        "1:",
                        "###",
                        "##.",
                        ".##",
                        "",
                        "2:",
                        ".##",
                        "###",
                        "##.",
                        "",
                        "3:",
                        "##.",
                        "###",
                        "##.",
                        "",
                        "4:",
                        "###",
                        "#..",
                        "###",
                        "",
                        "5:",
                        "###",
                        ".#.",
                        "###",
                        "",
                        "4x4: 0 0 0 0 2 0",
                        "12x5: 1 0 1 0 2 2",
                        "12x5: 1 0 1 0 3 2"
                    };
            
                    var result = Puzzle01.Solve(input);
                    Assert.Equal(2, result);
                }
            }