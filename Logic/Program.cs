using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    class Program
    {
        static void Main(string[] args)
        {
            //var f = new PlayingField(2);
            //f.array= new bool?[,]{{true, false},{true, false}};
            //f.NumSet = 4;
            //var mm= new bool[,]{ { true, false },{true, false}};

            //var h = f.GenerateHints(mm);

            //var b = f.IsSolution(h);

            //var hints = GenerateHints(@"kyor.png", 240);

            var hints1 = @"7 2 1 7
1 1 1 1 1 1
1 3 1 1 1 1 1 3 1
1 3 1 1 1 1 3 1
1 3 1 3 1 3 1
1 1 1 1
7 1 1 1 7
2
4 1 1 1 1 3 1
1 3 1 1 3 1
6 1 1 1 2
1 1 1 3 2 3
1 6 2 1 1
3 1 2
7 2 2 1 1
1 1 5 2
1 3 1 1 1 2 2
1 3 1 2 1 1 1 1
1 3 1 4 1 2
1 1 1 3 1 1 1
7 5 1 1";
            var hintsHorz = @"7 1 1 7
1 1 1 1 1 1
1 3 1 2 1 1 3 1
1 3 1 1 2 1 3 1
1 3 1 1 1 1 3 1
1 1 3 1 1
7 1 1 1 7
4
1 3 1 4 1 4
2 1 1 3 2 3 1
1 1 3 3 3
1 1 2 3
3 1 3 1 2
1 1 3 2
7 2 3 1
1 1 3 3 1
1 3 1 2 1 2 1
1 3 1 5 4
1 3 1 2 3 1 1
1 1 1 1
7 3 1";
            var hints = hints1.Split('\n').Select(x => x.Split(' ').Select(y => int.Parse(y)).ToList()).ToList()
                .Concat(hintsHorz.Split('\n').Select(x => x.Split(' ').Select(y => int.Parse(y)).ToList()).ToList()).ToList();
            List<string> lines = FormatHintsForExcel(hints, null);

            File.WriteAllLines("c:\\logic.txt", lines);
            Console.SetWindowSize(200, 94);
            var m = hints.Take(hints.Count / 2).Max(x => x.Sum(x2 => (x2 >= 10 ? 2 : 1) + 1));

           
            var iter = 0;
            int emptyEntries = 0;
            while (emptyEntries < hints.Count / 2)
            {
                for (int i = 0; i < m; i++)
                {
                    Console.Write(" ");
                }
                emptyEntries = 0;
                for (int i = 0; i < hints.Count/2; i++)
                {                               
                    if (hints[hints.Count / 2 + i].Count > iter)
                    {
                        Console.Write(hints[hints.Count / 2 + i][iter]);
                    }
                    else
                    {
                        Console.Write(" ");
                        emptyEntries++;
                    }
                }
                Console.Write("\n");
                iter++;
            }
            for (int i = 0; i < hints.Count / 2; i++)
            {
                foreach (var item in hints[i])
                {
                    Console.Write(item);
                    Console.Write(" ");
                }
                Console.Write("\n");
            }

            Console.WriteLine("Press any key to solve");
            Console.Read();

            var solver = new Solver(hints);
            
            var solutions = solver.Solve();

            for (int i = 0; i < solutions.Count; i++)
            {
                var res = FormatHintsForExcel(hints, solutions[i]);

                File.WriteAllLines($"c:\\logic{i}.txt", res);
            }
            Console.Read();
        }

        private static List<string> FormatHintsForExcel(List<List<int>> hints, PlayingField field)
        {
            var l = hints.Count/2;
            var max1 = hints.Take(l).Select(x => x.Count).Max();
            var max2 = hints.Skip(l).Select(x => x.Count).Max();

            var arr = new string[l + max2, l + max1];
            for (int i = 0; i < l; i++)
            {
                for (int j = 0; j < hints[i].Count(); j++)
                {
                    arr[max2 + i, max1 - hints[i].Count() + j] = hints[i][j].ToString();
                }
            }
            for (int i = 0; i < l; i++)
            {
                for (int j = 0; j < hints[i + l].Count(); j++)
                {
                    arr[max2 - hints[i + l].Count() + j, max1 + i] = hints[i + l][j].ToString();
                }
            }
            for (int i = 0; i < l; i++)
            {
                for (int j = 0; j < l; j++)
                {
                    if (field != null)
                        arr[max2 + j, max1 + i ] = field.array[j, i].HasValue ? (field.array[j,i].Value ? "X" : " ") : "?";
                }
            }
            var lines = new List<string>();
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                var line = "";
                var line2 = "";
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    if (arr[i, j] != null)
                        line += arr[i, j];
                    line += "\t";
                    if (arr[i, j] != null)
                        line2 += arr[i, j];

                    line2 += int.TryParse(arr[i, j],out  var n) ? (n > 9 ? " " : "  ") : "";
                }
                lines.Add(line);
            }
            return lines;
        }

        bool[,] Mask(string path, int threshold)
        {
            Bitmap img = (Bitmap) Bitmap.FromFile(path);
            Func<Color, bool> test = c => (c.R + c.G + c.B) < threshold * 3;
            var res = new bool[img.Height, img.Width];
            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0;j < img.Width; j++)
                {
                    var c = img.GetPixel(j, i);
                    if (test(c))
                    {
                        res[i, j] = true;
                    }
                }
            }

            return res;
        }

        
    }
}
