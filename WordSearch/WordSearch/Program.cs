using System;
using System.Collections.Generic;
using System.Data;

namespace WordSearch
{
    class Program
    {
        static char[,] Grid = new char[,] {
            {'C', 'P', 'K', 'X', 'O', 'I', 'G', 'H', 'S', 'F', 'C', 'H'},
            {'Y', 'G', 'W', 'R', 'I', 'A', 'H', 'C', 'Q', 'R', 'X', 'K'},
            {'M', 'A', 'X', 'I', 'M', 'I', 'Z', 'A', 'T', 'I', 'O', 'N'},
            {'E', 'T', 'W', 'Z', 'N', 'L', 'W', 'G', 'E', 'D', 'Y', 'W'},
            {'M', 'C', 'L', 'E', 'L', 'D', 'N', 'V', 'L', 'G', 'P', 'T'},
            {'O', 'J', 'A', 'A', 'V', 'I', 'O', 'T', 'E', 'E', 'P', 'X'},
            {'C', 'D', 'B', 'P', 'H', 'I', 'A', 'W', 'V', 'X', 'U', 'I'},
            {'L', 'G', 'O', 'S', 'S', 'B', 'R', 'Q', 'I', 'A', 'P', 'K'},
            {'E', 'O', 'I', 'G', 'L', 'P', 'S', 'D', 'S', 'F', 'W', 'P'},
            {'W', 'F', 'K', 'E', 'G', 'O', 'L', 'F', 'I', 'F', 'R', 'S'},
            {'O', 'T', 'R', 'U', 'O', 'C', 'D', 'O', 'O', 'F', 'T', 'P'},
            {'C', 'A', 'R', 'P', 'E', 'T', 'R', 'W', 'N', 'G', 'V', 'Z'}
        };

        static string[] Words = new string[] 
        {
            "CARPET",
            "CHAIR",
            "DOG",
            "BALL",
            "DRIVEWAY",
            "FISHING",
            "FOODCOURT",
            "FRIDGE",
            "GOLF",
            "MAXIMIZATION",
            "PUPPY",
            "SPACE",
            "TABLE",
            "TELEVISION",
            "WELCOME",
            "WINDOW"
        };
        
        static void Main(string[] args)
        {
            Console.WriteLine("Word Search");

            for (int y = 0; y < 12; y++)
            {
                for (int x = 0; x < 12; x++)
                {
                    Console.Write($"{Grid[y, x]}-{x},{y}");
                    Console.Write(' ');
                }
                Console.WriteLine("");

            }

            Console.WriteLine("");
            Console.WriteLine("Found Words");
            Console.WriteLine("------------------------------");

            FindWords();

            Console.WriteLine("------------------------------");
            Console.WriteLine("");
            Console.WriteLine("Press any key to end");
            Console.ReadKey();
        }

        private static void FindWords()
        {
            //Find each of the words in the grid, outputting the start and end location of
            //each word, e.g.
            //PUPPY found at (10,7) to (10, 3)
            var wordTracking = new HashSet<string>(); 
            for (int y = 0; y < 12; y++)
            {
                for (int x = 0; x < 12; x++)
                {
                    foreach (var word in Words)
                    {
                        if (Grid[y, x] != word[0]) //Word not found
                            continue;
                        else if (wordTracking.Contains(word))
                            continue; //already found

                        var isWordFound = FindAWord(y,x, word);
                        if (isWordFound)
                        {
                            wordTracking.Add(word); 
                        }
                    }
                }
            }
          
        }

        private static bool FindAWord(int x, int y, string word)
        {
            //for each letter in the word
            //go through all 8 directions
            // if not found, go to the next word
            //if found output the start and end location
            //directions:
            // x-1,y-1 | x+0,y-1 | x+1,y-1 
            // x-1,y+0 | x,y     | x+1,y+0
            // x-1,y+1 | x+0,y+1 | x+1,y+1
            int[] xDirs = {-1, 0, 1, -1, 1, -1, 0, 1};
            int[] yDirs = {-1, -1, -1, 0, 0, 1, 1, 1};
            var numberOfColumn = Grid.GetLength(1);
            var numberOfRows = Grid.GetLength(0);


            var startLocation = new Location() {X = x, Y = y};
            for (var dir = 0; dir < 8; dir++)
            {
                var xDirection = x + xDirs[dir];
                var yDirection = y + yDirs[dir];

                //test is not part of the rqeuirements
                //Test cases: test boundaries: Grid(-1,-1), Grid(12, 12) 

                int letterPositionInWord = 0;
                for (letterPositionInWord = 1; letterPositionInWord < word.Length; letterPositionInWord++)
                {
                    if (xDirection < 0 || xDirection >= numberOfColumn || yDirection < 0 || yDirection >= numberOfRows)
                        break; //row out of bound
                    ;
                    if (Grid[xDirection, yDirection] !=
                        word[letterPositionInWord]) //explore new direction, until all 8 directions
                        break;

                    //Console.WriteLine($"found word {word} x={xDirection}, y={yDirection}, letter={word[letterPositionInWord]}. LetterPos={letterPositionInWord}, len={word.Length}");
                    //continue to explore in the same direction until all letters are found 
                    if (letterPositionInWord == (word.Length - 1))
                    {
                        var endLocation = new Location() {X = xDirection, Y = yDirection};
                        Console.WriteLine($"====Word found {word}, start at ({startLocation.Y}, {startLocation.X}), end at ({endLocation.Y}, {endLocation.X})\n");
                        return true; 
                    }
                    
                    xDirection += xDirs[dir];
                    yDirection += yDirs[dir];
                }
            }

            return false;
        }
    }
}
