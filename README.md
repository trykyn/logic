# logic
logic solver - a simple command line tool to solve or generate games of logic (also known as nonogram, pricross or griddlers)

# Installation 

git clone, then run in visual studio

# Usage

Can either generate a game of logic or solve one. 

## Generation:
```
var hints = GenerateHints(@"kyor.png", 240);
List<string> lines = FormatHintsForExcel(hints, null);
File.WriteAllLines("c:\\logic.txt", lines);
```
  
## Solving
Enter the hints in the arrays hints1 and hintsHorz and run  
```
var solver = new Solver(hints);           
var solutions = solver.Solve();
```
The solver can figure out non-unique solutions using backtracking, the result will be an array of solutions
