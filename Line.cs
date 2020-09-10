using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace NonogramResolver
{
    class Line
    {
        public List<int> Band { get; set; } = new List<int>();
        public readonly int goal;
        public readonly int pos;
        public readonly bool isRow;
        private int min = 0, max = 0;

        public Line(string l, int p, bool ir)
        {
            if (!l.Equals("0")) Band = l.Split(Consts.numSeparator).ToList().ConvertAll(int.Parse);
            goal = Band.Sum();
            pos = p;
            isRow = ir;
        }
        public void CreateButton(Dimensions dim, Form form)
        {
            max = (isRow) ? dim.VerticalLength : dim.HorizontalLength;
            int maxButtons = (isRow) ? dim.VerticalMax : dim.HorizontalMax;
            for (int i = 0; i < Band.Count; i++)
            {
                Button myBtn = Consts.GetButtonSkeletron(dim);
                myBtn.BackColor = Color.Beige;
                myBtn.Text = Band[i].ToString();
                int x = (isRow) ? (10 + (Consts.GetSize(dim) + 1) * i) : (10 + (Consts.GetSize(dim) + 1) * maxButtons + (Consts.GetSize(dim) + 1) * pos);
                int y = (isRow) ? (10 + (Consts.GetSize(dim) + 1) * maxButtons + (Consts.GetSize(dim) + 1) * pos) : (10 + (Consts.GetSize(dim) + 1) * i);
                myBtn.Location = new System.Drawing.Point(x, y);
                form.Controls.Add(myBtn);
            }
        }

        public bool RemoveObvious(Dimensions dim, Grid grid)
        {
            int length = ((isRow) ? dim.VerticalLength : dim.HorizontalLength);
            bool empty = goal == 0;
            bool full = goal == length;
            if (empty) grid.UpdateLine(pos, isRow, State.White);
            if (full) grid.UpdateLine(pos, isRow, State.Black);
            return empty || full;
        }

        public void FillObviousBlack(Dimensions dim, Grid grid)
        {
            int diff = max - (Band.Count + Band.Sum() - 1);
            int counter = 0;
            foreach (int value in Band)
            {
                int sub = value - diff;
                int tempPos = counter + diff + min;
                if (sub > 0) grid.UpdateLine(pos, tempPos, sub, isRow, State.Black);
                counter += value + 1;
            }
        }
        public bool FillBorders(Grid grid, bool isLeft)
        {
            if (Band.Count == 0 || goal == grid.GetBlacks(pos, isRow))
            {
                grid.FillLine(pos, isRow, State.White);
                return true;
            }
            State borderState = grid.GetPointState(pos, isRow, (isLeft) ? min : (max - 1));
            if (borderState == State.White)
            {
                min += Convert.ToInt32(isLeft);
                max -= Convert.ToInt32(!isLeft);
                return FillBorders(grid, isLeft);
            }
            if (borderState == State.Black)
            {
                if (isLeft) grid.UpdateLine(pos, min, Band.First(), isRow, State.Black);
                if (!isLeft) grid.UpdateLine(pos, max - Band.Last(), Band.Last(), isRow, State.Black);
                if (isLeft && (min + Band[0] < max)) grid.UpdatePoint(min + Band.First(), pos, isRow, State.White);
                if (!isLeft && (max - Band.Last() > min)) grid.UpdatePoint(max - Band.Last() - 1, pos, isRow, State.White);
                min += (Band.First() + 1) * Convert.ToInt32(isLeft);
                max -= (Band.Last() + 1) * Convert.ToInt32(!isLeft);
                Band.RemoveAt((isLeft) ? 0 : Band.Count - 1);
                return FillBorders(grid, isLeft);
            }
            return false;
        }

        public void PartialFill(Grid grid, bool isLeft)
        {
            int firstWhiteIndex = grid.GetFirstWhiteIndex(pos, isRow, isLeft, (isLeft) ? min : max);
            int firstBlackIndex = grid.GetFirstBlackIndex(pos, isRow, isLeft, (isLeft) ? min : max);
            if (firstWhiteIndex == -1) firstWhiteIndex = (isLeft) ? max : min - 1;
            int diff = (isLeft) ? firstWhiteIndex - min : (max - 1) - firstWhiteIndex;
            if (diff < ((isLeft) ? Band.First() : Band.Last()))
            {
                grid.UpdateLine(pos, (isLeft) ? min : firstWhiteIndex + 1, diff, isRow, State.White);
                if (isLeft) min = firstWhiteIndex + 1;
                if (!isLeft) max = firstWhiteIndex;
            }
            else if (diff < 2 * ((isLeft) ? Band.First() : Band.Last()))
            {
                if (((isLeft) ? firstBlackIndex <= firstWhiteIndex : firstBlackIndex >= firstWhiteIndex) && firstBlackIndex != -1)
                {
                    int val1 = (isLeft) ? firstWhiteIndex - Band.First() : max - Band.Last();
                    int val2 = ((isLeft) ? Band.First() : Band.Last()) * 2 - diff;
                    grid.UpdateLine(pos, val1, val2, isRow, State.Black);
                }
            }
        }
        public bool AttemptOne(Grid grid)
        {
            List<State> actualSituation = grid.GetLine(pos, isRow).GetRange(min, max - min).Select(e => e.State).ToList();
            List<string> possibilities = Consts.CoherentStrings(max - min, Band, new string(actualSituation.Select(e => (char)e).ToArray()));
            List<State> result = new List<State>();
            for (int i = 0; i < max - min; i++)
            {
                double xor = possibilities.Sum(e => (int)Char.GetNumericValue(e[i]));
                result.Add((xor == 0) ? State.White : (xor == possibilities.Count) ? State.Black : State.Unknown);
            }
            bool solFound = !result.SequenceEqual(actualSituation);
            if (solFound) for (int i = 0; i < max - min; i++) grid.UpdatePoint(i + min, pos, isRow, result[i]);
            return solFound;
        }

        public bool CheckOverflow(Dimensions dim)
        {
            return Band.Sum() <= ((isRow) ? dim.VerticalLength : dim.HorizontalLength);
        }
    }
}
