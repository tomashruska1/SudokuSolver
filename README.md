# SudokuSolver

C# Sudoku solver that combines recursive and non-recursive technique

First it attempts to solve the board non-recursively using dictionaries and hash sets, looking for cells with a single possible value.

If it is unable to solve the board this way, it calls the recursive function to finish the board.
