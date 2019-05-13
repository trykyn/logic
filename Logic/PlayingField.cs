using System;
using System.Collections.Generic;
using System.Linq;

namespace Logic
{
    class PlayingField
    {
        public int NumSet { get; set; }
        public bool?[,] array { get;  set; }
        public int Size { get; private set; }
        public PlayingField(int n)
        {
            this.array = new bool?[n, n];
            this.Size = n;
            this.NumSet = 0;
        }

        public PlayingField Copy()
        {
            var res = new PlayingField(Size);
            res.NumSet = NumSet;
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    res.array[i, j] = array[i, j];
                }
            }

            return res;
        }

        public void Set(int n, int i, bool? value, int[] hotness = null)
        {
            if (n < Size)
            {
                if (value != null && array[n, i] == null)
                {
                    array[n, i] = value;
                    if (hotness != null)
                        hotness[i + Size] = hotness[i + Size] + 1;
                    NumSet++;
                }
                if (hotness != null)
                    hotness[n] = 0;
            }
            else
            {
                if (value != null && array[i, n - Size] == null)
                {
                    array[i, n - Size] = value;
                    if (hotness != null)
                        hotness[i] = hotness[i] + 1;
                    NumSet++;
                }
                if (hotness != null)
                    hotness[n] = 0;
            }
        }
        public void Print()
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    if (array[i, j] == false)
                        Console.Write("█");
                    else if (array[i, j] == null)
                        Console.Write(".");
                    else
                        Console.Write(" ");
                }
                Console.WriteLine();
            }
        }

        public bool IsFeasibleSolution(int n, bool[] arr)
        {
            if (n < Size) // row
            {
                for (int i = 0; i < Size; i++)
                {
                    if (array[n, i].HasValue && array[n, i] != arr[i])
                        return false;
                }
                return true;
            }
            else // col
            {
                for (int i = 0; i < Size; i++)
                {
                    if (array[i, n - Size].HasValue && array[i, n - Size] != arr[i])
                        return false;
                }
                return true;
            }
        }

        public bool?[] Intersect(IEnumerable<bool[]> candidates)
        {
            if (!candidates.Any())
                throw new Exception("Puzzle seems to be wrong!");

            var res = new bool?[Size];
            for (int i = 0; i < Size; i++)
            {
                res[i] = candidates.First()[i];
            }
            foreach (var cand in candidates)
            {
                for (int i = 0; i < Size; i++)
                {
                    if (cand[i] != res[i] && res[i] != null)
                        res[i] = null;
                }
            }
            return res;
        }

        public bool IsSolution(List<List<int>> givenHints)
        {
            if (NumSet != Size * Size)
                return false;
            var mask = new bool[Size, Size];
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    mask[i, j] = array[i, j].Value;
                }
            }

            var hints = GenerateHints(mask);

            var blubb = givenHints.Zip(hints, (g, s) => g.Count == s.Count && g.Zip(s, (a, b) => a == b).All(z => z == true));

            if (blubb.All(x=>x==true))
            {
                return true;
            }

            return false;
        }

        public List<List<int>> GenerateHints(bool[,] mask)
        {
            var res = new List<List<int>>();
            var height = mask.GetLength(0);
            var width = mask.GetLength(1);
            for (int i = 0; i < height; i++)
            {
                var l = new List<int>();
                bool isBlack = false;
                var currentHint = 0;
                for (int j = 0; j < width; j++)
                {
                    if (mask[i, j])
                    {
                        // dark
                        if (isBlack)
                            currentHint++;
                        else
                        {
                            isBlack = true;
                            currentHint = 1;
                        }
                    }
                    else
                    {
                        if (isBlack)
                        {
                            if (currentHint > 0)
                                l.Add(currentHint);
                            currentHint = 0;
                            isBlack = false;
                        }
                        else
                        {
                            ; // NOTHING
                        }
                        // light
                    }
                }
                if (currentHint > 0 || (!l.Any() && currentHint == 0))
                    l.Add(currentHint);
                res.Add(l);
            }
            for (int i = 0; i < width; i++)
            {
                var l = new List<int>();
                bool isBlack = false;
                var currentHint = 0;
                for (int j = 0; j < height; j++)
                {
                    if (mask[j, i])
                    {
                        // dark
                        if (isBlack)
                            currentHint++;
                        else
                        {
                            isBlack = true;
                            currentHint = 1;
                        }
                    }
                    else
                    {
                        if (isBlack)
                        {
                            if (currentHint > 0)
                                l.Add(currentHint);
                            currentHint = 0;
                            isBlack = false;
                        }
                        else
                        {
                            ; // NOTHING
                        }
                        // light
                    }
                }
                if (currentHint > 0 || (!l.Any() && currentHint == 0))
                    l.Add(currentHint);
                res.Add(l);
            }
            return res;
        }


    }


}