using System;
using System.Collections.Generic;
using System.Linq;

namespace Logic
{
    class Solver
    {
        public PlayingField field;
        List<List<int>> Hints;
        public int Size { get { return Hints.Count / 2; } }
        public Solver(List<List<int>> hints)
        {
            this.Hints = hints;
            this.field = new PlayingField(hints.Count / 2);
        }

        Dictionary<string, IEnumerable<bool[]>> cache = new Dictionary<string, IEnumerable<bool[]>>();
        public List<PlayingField> Solve()
        {
            var iter = 0;


            var m = new Queue<int>();
            for (int i = 0; i < field.Size * 2; i++)
            {
                m.Enqueue(int.MinValue);
            }
            var hotness = Hints.Select(x => x.Count == 1 && x.First() == 0 ? int.MaxValue : x.Sum() + x.Count).ToArray();

            while (field.NumSet < field.Size * field.Size && m.Peek() < field.NumSet)
            {
                var maxHot = hotness.Max();
                var hotCursors = hotness.Select((x, ind) => new { x, ind }).Where(x => x.x == maxHot).Select(x => x.ind).ToArray();
                foreach (var cursor in hotCursors)
                {
                    // Generate solutions from hints
                    IEnumerable<bool[]> candidates;
                    if (!cache.TryGetValue(String.Join(",", Hints[cursor]), out candidates))
                    {
                        candidates = GenerateCandidates(Hints[cursor]);
                        cache[String.Join(",", Hints[cursor])] = candidates;
                    }

                    var common = field.Intersect(candidates.Where(c => field.IsFeasibleSolution(cursor, c)));

                    for (int i = 0; i < Size; i++)
                    {
                        field.Set(cursor, i, common[i], hotness);
                    }
                    Console.WriteLine("Iteration " + iter);
                    field.Print();
                    iter++;
                    m.Dequeue();
                    m.Enqueue(field.NumSet);
                }

            }
            // start backtracking empty values
            var unsolvedPos = new List<(int, int)>();
            for (int i = 0; i < field.Size; i++)
            {
                for (int j = 0; j < field.Size; j++)
                {
                    if (field.array[i, j] == null)
                        unsolvedPos.Add((i, j));
                }
            }

            var playingFields = new List<PlayingField>();
            Backtrack(unsolvedPos, field, playingFields);


            return playingFields;

            //Console.WriteLine("Finished");
        }

        public void Backtrack(List<(int, int)> unsolvedPos, PlayingField f, List<PlayingField> solutions)
        {
            if (unsolvedPos.Count == 0)
            {

                if (f.IsSolution(Hints))
                {
                    solutions.Add(f);
                    Console.WriteLine("Found a solutions");
                }
                else
                {
                    Console.Write(".");
                }

                return;
            }

            foreach (var b in new[] { false, true })
            {
                var playingField = f.Copy();
                playingField.Set(unsolvedPos[0].Item1, unsolvedPos[0].Item2, b, null);
                
                Backtrack(unsolvedPos.Skip(1).ToList(), playingField, solutions);
            }

        }
        IEnumerable<bool[]> GenerateCandidates(List<int> hint, bool[] currentCandidate = null, int hintCursor = 0, int placementCursor = 0, bool isFirst = false)
        {
            if (hint.Count == 1 && hint[0] == 0)
                return new[] { new bool[Size] };

            var rest = hint.Skip(hintCursor).Skip(1);
            var currentHint = hint[hintCursor];
            var reqForRest = rest.Any() ? rest.Sum() + rest.Count() : 0; // total count, plus each starts with at least 1 empty

            var remainingSlack = Size - placementCursor - reqForRest - currentHint;

            return Enumerable.Range(placementCursor, remainingSlack + 1).SelectMany(placementStart =>
              {
                  bool[] newCandidate;
                  if (isFirst && currentCandidate != null)
                      newCandidate = currentCandidate;
                  else
                  {
                      newCandidate = new bool[Size];
                      if (placementCursor > 0) // copy over previous values
                          for (int i = 0; i < placementCursor; i++)
                              newCandidate[i] = currentCandidate[i];
                  }
                  // place sequence according to current hint
                  for (int i = 0; i < currentHint; i++)
                      newCandidate[placementStart + i] = true;

                  if (hintCursor == hint.Count - 1)
                      return new[] { newCandidate };
                  else
                      return GenerateCandidates(hint, newCandidate, hintCursor + 1, placementStart + currentHint + 1 /* empty field*/);
              });

        }

    }
}