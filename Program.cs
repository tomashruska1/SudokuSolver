using System;
using System.Collections.Generic;

namespace SudokuSolver
{

    class Program
    {
        static void Main()
        {
            // we initialize the class that is solving the sudoku

            SudokuSolver ss = new();

            // SetUp creates the board
            ss.SetUp();

            // and we try to solve the board
            if (!ss.Solve())
            {
                Console.WriteLine("Solution not found");
            }
        }
    }

    class SudokuSolver
    {
        public int[,] Board = new int[9, 9];

        public void SetUp()
        {
            // an example board, Sudoku boards can be easily converted into the below format using excel and below macro for example

            // Sub getSudoku()
            //     Dim cell As Range
            //     Dim sht As Worksheet
            //     Dim result As String
            //     result = ""
            //     Set sht = ThisWorkbook.Sheets(1)
            //     For Each cell In sht.Range("A1:I9")
            //         If cell.Value <> "" Then
            //             result = result & "Board[" & cell.Row - 1 & ", " & cell.Column - 1 & "] = " & cell.Value & ";" & vbCrLf
            //         End If
            //     Next cell
            //     sht.Range("M1").Value = result
            // End Sub


            Board[0, 0] = 3;
            Board[0, 5] = 9;
            Board[0, 6] = 7;
            Board[0, 8] = 1;
            Board[1, 1] = 8;
            Board[1, 3] = 3;
            Board[2, 2] = 5;
            Board[2, 5] = 6;
            Board[2, 6] = 8;
            Board[2, 7] = 3;
            Board[3, 7] = 9;
            Board[3, 8] = 8;
            Board[4, 2] = 7;
            Board[4, 6] = 4;
            Board[5, 0] = 5;
            Board[5, 1] = 3;
            Board[6, 1] = 4;
            Board[6, 2] = 3;
            Board[6, 3] = 6;
            Board[6, 6] = 1;
            Board[7, 5] = 3;
            Board[7, 7] = 2;
            Board[8, 0] = 1;
            Board[8, 2] = 8;
            Board[8, 3] = 4;
            Board[8, 8] = 5;


        }

        public bool Solve()
        {
            return Solve(Board);
        }

        private static bool Solve(int[,] board)
        {
            // We use several dictionaries that contain possible values in HashSets, we attempt to solve the sudoku non-recursively
            // by checking when does a cell have only one possible value

            Dictionary<int, HashSet<int>> Rows = new();
            Dictionary<int, HashSet<int>> Columns = new();
            Dictionary<(int, int), HashSet<int>> Squares = new();
            Dictionary<(int, int), HashSet<int>> PossibleMatches = new();

            // we populate the dictionaries
            FillDictionaries(board, Rows, Columns, Squares, PossibleMatches);

            // bool changed makes sure we do not get stuck in a forever loop
            bool changed;

            do
            {
                changed = false;
                int squareRow;
                int squareColumn;

                for (int row = 0; row < 9; row++)
                {
                    squareRow = row / 3;

                    for (int column = 0; column < 9; column++)
                    {
                        squareColumn = column / 3;

                        // empty board[row, column] returns zero, if the cell is not empty, we continue with the next iteration
                        if (board[row, column] != 0)
                        {
                            continue;
                        }

                        // we check whether any values in the PossibleMatches dictionary are no longer valid and remove them if so
                        if (PossibleMatches[(row, column)].Count > 0)
                        {

                            foreach (int possibleMatch in PossibleMatches[(row, column)])
                            {
                                if (!Rows[row].Contains(possibleMatch) || !Columns[column].Contains(possibleMatch) || !Squares[(squareRow, squareColumn)].Contains(possibleMatch))
                                {
                                    PossibleMatches[(row, column)].Remove(possibleMatch);
                                }
                            }
                        }

                        // if the HashSet for the current (row, column) in the PossibleMatches has only one value, we enter it into the board
                        // and remove the number from all appropriate HashSets
                        if (PossibleMatches[(row, column)].Count == 1)
                        {
                            foreach (int number in PossibleMatches[(row, column)])
                            {
                                board[row, column] = number;
                                Rows[row].Remove(number);
                                Columns[column].Remove(number);
                                Squares[(squareRow, squareColumn)].Remove(number);
                                PossibleMatches[(row, column)].Remove(number);
                                changed = true;
                            }
                        }
                    }
                }

                // we check across all rows whether there are any numbers that can only be entered into one cell
                // if we find such number, we enter it and remove from all appropriate HashSets
                HashSet<int> rows = new() { };
                HashSet<int> columns = new() { };

                for (int row = 0; row < 9; row++)
                {
                    squareRow = row / 3;

                    for (int number = 1; number <= 9; number++) 
                    {
                        rows.Clear();

                        for (int column = 0; column < 9; column++)
                        {
                            squareColumn = column / 3;

                            if (board[row, column] == 0)
                            {
                                if (Rows[row].Contains(number) && Columns[column].Contains(number) && Squares[(squareRow, squareColumn)].Contains(number))
                                {
                                    rows.Add(column);
                                }
                            }
                        }

                        if (rows.Count == 1)
                        {
                            foreach (int setColumn in rows)
                            {
                                board[row, setColumn] = number;
                                Rows[row].Remove(number);
                                Columns[setColumn].Remove(number);
                                Squares[(row / 3, squareRow)].Remove(number);
                            }
                        }
                    }
                }

                // we do the same thing for all columns
                for (int column = 0; column < 9; column++)
                {
                    squareColumn = column / 3;

                    for (int number = 1; number <= 9; number++)
                    {
                        columns.Clear();

                        for (int row = 0; row < 9; row++)
                        {
                            squareRow = row / 3;

                            if (board[row, column] == 0)
                            {
                                if (Rows[row].Contains(number) && Columns[column].Contains(number) && Squares[(squareRow, squareColumn)].Contains(number))
                                {
                                    columns.Add(row);
                                }
                            }
                        }

                        if (columns.Count == 1)
                        {
                            foreach (int setRow in columns)
                            {
                                board[setRow, column] = number;
                                Rows[setRow].Remove(number);
                                Columns[column].Remove(number);
                                Squares[(setRow / 3, squareColumn)].Remove(number);
                            }
                        }
                    }
                }

            // we keep looping until we either solve the board or we can no longer find any new numbers this way
            } while (!CheckSolution(board) && changed);

            // in case we have not been able to find all numbersm, we attempt to solve the board recursively
            if (CheckSolution(board) || RecurseSolve(board))
            {
                // if we find a solution, we print it into the console
                Console.WriteLine("*****WINNER*****");
                PrintBoardToConsole(board);
                Console.WriteLine("*****WINNER*****");
                return true;
            }
            return false;
        }

        private static bool RecurseSolve(int[,] board)
        {
            // we iterate over the board to find the first empty cell
            for (int row = 0; row < 9; row++)
            {
                for (int column = 0; column < 9; column++)
                {

                    // we only try to enter into empty cells and skip non-empty ones
                    if (board[row, column] != 0)
                        continue;

                    // we attempt to enter a valid number and then recurse
                    for (int number = 1; number <= 9; number++)
                    {
                        if (IsValid(board, row, column, number))
                        {
                            board[row, column] = number;

                            // if we were able to find a solution deeper in the recursive calls,
                            // we return True, otherwise we delete the incorrect number and start-over
                            if (RecurseSolve(board))
                                return true;
                            else
                                board[row, column] = 0;
                        }
                    }

                    // if we reach this point, we were not able to find a valid number for an empty cell
                    return false;
                }
            }
            // if we reach this point, we have no more empty cells and we have solved this board
            return true;
        }

        private static bool IsValid(int[,] board, int row, int column, int number)
        {
            for (int n = 0; n < 9; n++)
            {
                // we check whether the current row contains the given number
                if (board[row, n] == number)
                    return false;

                // and the same for column
                if (board[n, column] == number)
                    return false;

                //  (row / 3) * 3  returns the first row in the smaller square, by adding modulo of the n, we only ever move up to two cells to the right
                //  (column / 3) * 3  likewise returns first column, to which we add the floor division of the n by 3, which increases the column once every 3 iterations
                if (board[(row / 3) * 3 + n % 3, (column / 3) * 3 + n / 3] == number)
                    return false;

            }

            return true;
        }

        private static void FillDictionaries(int[,] board, Dictionary<int, HashSet<int>> Rows, Dictionary<int, HashSet<int>> Columns, Dictionary<(int, int),
            HashSet<int>> Squares, Dictionary<(int, int), HashSet<int>> PossibleMatches)
        {

            // first we populate Rows dictionary and initialize the PossibleMatches dictionary
            for (int row = 0; row < 9; row++)
            {
                // for each row on the board, we add number of the row as a kay with a new HashSet as value, we initialize this with all possible values
                Rows.Add(row, new HashSet<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
                for (int column = 0; column < 9; column++)
                {
                    // we initialize the PossibleMatches dictionary below
                    PossibleMatches.Add((row, column), new HashSet<int> { });

                    // we remove any values already on the board in the same row
                    if (board[row, column] != 0)
                    {
                        Rows[row].Remove(board[row, column]);
                    }
                }
            }

            // we populate the Columns dictionary with all 9 possible values
            for (int column = 0; column < 9; column++)
            {
                Columns.Add(column, new HashSet<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
                for (int row = 0; row < 9; row++)
                {
                    if (board[row, column] != 0)
                    {
                        // and remove any already on the board
                        Columns[column].Remove(board[row, column]);
                    }
                }
            }

            // then we populate the Dictionary for the smaller squares
            for (int bigRow = 0; bigRow < 3; bigRow++)
            {
                for (int bigColumn = 0; bigColumn < 3; bigColumn++)
                {
                    Squares.Add((bigRow, bigColumn), new HashSet<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
                    for (int row = bigRow * 3; row < bigRow * 3 + 3; row++)
                    {
                        for (int column = bigColumn * 3; column < bigColumn * 3 + 3; column++)
                        {
                            if (board[row, column] != 0)
                            {
                                // and then we remove all values already on the board within the given square
                                Squares[(bigRow, bigColumn)].Remove(board[row, column]);
                            }
                        }
                    }
                }
            }

            int squareRow;
            int squareColumn;

            // lastly we populate the PossibleMatches dictionary, to this one we are instead adding values that are not yet on the board
            // for the given row, column and square
            for (int row = 0; row < 9; row++)
            {
                squareRow = row / 3;

                for (int column = 0; column < 9; column++)
                {
                    squareColumn = column / 3;

                    // we skip already filled in cells
                    if (board[row, column] != 0)
                    {
                        continue;
                    }

                    // we check all numbers and add those that are not on the board -> we check that the given number is still possible for the current row, column and square
                    for (int number = 1; number <= 9; number++)
                    {
                        if (Rows[row].Contains(number) && Columns[column].Contains(number) && Squares[(squareRow, squareColumn)].Contains(number))
                        {
                            PossibleMatches[(row, column)].Add(number);
                        }
                    }

                }
            }
        }

        private static bool CheckSolution(int[,] board)
        {
            HashSet<int> set = new();
            int row;
            int column;
            int rowRange;
            int colRange;

            // first we check all rows by adding numbers to a HashSet, if the number is already in the HashSet, the operation returns False and we have a duplicate
            // we also check that no cell contains 0, which is the default value
            for (row = 0; row < 9; row++)
            {
                set.Clear();
                for (column = 0; column < 9; column++)
                {
                    if (!set.Add(board[row, column]) || board[row, column] == 0)
                    {
                        return false;
                    }
                }
            }

            // we then check all columns
            for (column = 0; column < 9; column++)
            {
                set.Clear();
                for (row = 0; row < 9; row++)
                {
                    if (!set.Add(board[row, column]))
                    {
                        return false;
                    }
                }
            }

            // and squares
            for (int rowCoeff = 1; rowCoeff <= 3; rowCoeff++)
            {
                rowRange = 3 * rowCoeff;
                for (int colCoeff = 1; colCoeff <= 3; colCoeff++)
                {
                    colRange = 3 * colCoeff;
                    for (row = rowRange - 3; row < rowRange; row++)
                    {
                        set.Clear();
                        for (column = colRange - 3; column < colRange; column++)
                        {
                            if (!set.Add(board[row, column]))
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            // if we have reached this point, it means no HashSet.Add operation has failed and we have solved the board
            return true;
        }

        public static void PrintBoardToConsole(int[,] board)
        {
            Console.WriteLine(" _____________________");
            for (int row = 0; row < 9; row++)
            {
                Console.Write("| ");
                for (int column = 0; column < 9; column++)
                {
                    if (column == 3 || column == 6)
                    {
                        Console.Write("|");
                    }
                    Console.Write($"{board[row, column]} ");
                }
                Console.WriteLine("|");
                if (row == 2 || row == 5)
                {
                    Console.WriteLine(" ---------------------");
                }
            }
            Console.WriteLine(" _____________________\r\n");

        }

    }
}
