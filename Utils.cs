using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace NonogramResolver
{
    public enum State
    {
        Black = '1',
        White = '0',
        Unknown = '2'
    }

    public class Consts
    {
        public static readonly string url = "https://webpbn.com/export.cgi?fmt=makhorin&go=1&id=";
        public static readonly string[] dirSeparator = new string[] { "\n&\n" };
        public static readonly string[] lineSeparator = new string[] { "\n" };
        public const char numSeparator = ' ';

        public static int GetSize(Dimensions dim)
        {
            return 30 - ((dim.HorizontalLength > 25) ? 2 * dim.HorizontalLength / 10 : 0);
        }

        public static Button GetButtonSkeletron(Dimensions dim)
        {
            Button myBtn = new Button();
            myBtn.Size = new Size(GetSize(dim), GetSize(dim));
            myBtn.FlatStyle = FlatStyle.Flat;
            myBtn.FlatAppearance.BorderColor = Color.Black;
            myBtn.Enabled = false;
            myBtn.Font = new Font(FontFamily.GenericSansSerif, 8 - ((dim.HorizontalLength > 25) ? (dim.HorizontalLength / 10) - 1: 0 ));
            return myBtn;
        }

        static bool ConstrainRespected(string constraint,string value, int from, int count)
        {
            var zip = constraint.Zip(value, Tuple.Create).ToList().GetRange(from,count);
            return zip.TrueForAll(e => e.Item1 == '2' || e.Item1 == e.Item2);
        }

        public static List<string> CoherentStrings(int length, List<int> line,string check)
        {
            List<List<string>> possibilitiesSplitted = new List<List<string>>();
            for (int i = 0; i < line.Count; i++)
            {
                List<string> combo = new List<string>();
                int sum = length - line.Sum() + line[i] - line.Count + 1;
                for (int j = 0; j < (sum - line[i] + 1); j++)
                {
                    int left = line.Where((e, x) => x < i).Sum(e => e + 1);
                    string poss = new string('0', j + left) + new string('1', line[i]) + new string('0', length - line[i] - j - left);
                    if (ConstrainRespected(check, poss,j + left, line[i])) combo.Add(poss);
                }
                possibilitiesSplitted.Add(combo);
            }
            List<string> possibilities = GetCombos(possibilitiesSplitted);
            possibilities.RemoveAll(e => !e.Split('0').ToList().Select(x => x.Length).Where(x => x > 0).ToList().SequenceEqual(line) || !ConstrainRespected(check,e,0,length));
            return possibilities;
        }

        static List<string> GetCombos(List<List<string>> remainingTags)
        {
            if (remainingTags.Count() == 1) return remainingTags.First();
            var current = remainingTags.First();
            List<string> outputs = new List<string>();
            List<string> combos = GetCombos(remainingTags.Where((e,i) => i > 0).ToList());
            foreach (var tagPart in current) foreach (var combo in combos)
                {
                    string sum = tagPart.Zip(combo, (a, b) => (int)(Char.GetNumericValue(a) + Char.GetNumericValue(b))).Select(e => Convert.ToString(e)).Aggregate((a,b) => a + b);
                    if (!sum.Contains('2')) outputs.Add(sum);
                }
            return outputs;
        }
    }

    public class Level
    {
        public Color Color { get; set; } = Color.Transparent;

        State state = State.Unknown;
        public State State
        {
            get { return state; }
            set
            {
                state = value;
                Color = (state.Equals(State.Black)) ? Color.Black : (state.Equals(State.White) ? Color.White : Color);
            }
        }
    }

    public class Dimensions
    {
        public int HorizontalLength { get; }
        public int VerticalLength { get; }
        public int HorizontalMax { get; }
        public int VerticalMax { get; }

        public Dimensions(int hl, int vl, int hm, int vm)
        {
            HorizontalLength = hl;
            VerticalLength = vl;
            HorizontalMax = hm;
            VerticalMax = vm;
        }

    }
}
