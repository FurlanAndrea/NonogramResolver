using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

namespace NonogramResolver
{
    class Grid
    {
        public List<List<Point>> grid = new List<List<Point>>();

        public void CreateGrid(Form form, Dimensions dim)
        {
            for (int i = 0; i < dim.HorizontalLength; i++)
            {
                List<Point> line = new List<Point>();
                for (int j = 0; j < dim.VerticalLength; j++)
                {
                    Button mybtn = Consts.GetButtonSkeletron(dim);
                    mybtn.Location = new System.Drawing.Point(10 + (Consts.GetSize(dim) + 1) * j + (Consts.GetSize(dim) + 1) * dim.HorizontalMax, 10 + (Consts.GetSize(dim) + 1) * i + (Consts.GetSize(dim) + 1) * dim.VerticalMax);
                    form.Controls.Add(mybtn);
                    line.Add(new Point(mybtn));
                }
                grid.Add(line);
            }
        }
        public List<Point> GetLine(int pos, bool isRow)
        {
            return (isRow) ? grid[pos] : grid.Select(e => e[pos]).ToList();
        }
        public State GetPointState(int pos, bool isRow, int value)
        {
            return GetLine(pos, isRow)[value].State;
        }
        public int GetBlacks()
        {
            return grid.Sum(x => x.Sum(y => (y.State == State.Black ? 1 : 0)));
        }
        public int GetBlacks(int pos, bool isRow)
        {
            return GetLine(pos, isRow).Sum(x => (x.State == State.Black) ? 1 : 0);
        }
        public int GetWhites()
        {
            return grid.Sum(x => x.Sum(y => (y.State == State.White ? 1 : 0)));
        }
        public int GetUnknowns(int pos, bool isRow)
        {
            return GetLine(pos, isRow).Sum(x => (x.State == State.Unknown) ? 1 : 0);
        }
        public void UpdatePoint(int x, int y, bool isRow, State state)
        {
            grid[(isRow) ? y : x][(isRow) ? x : y].State = state;
        }
        public void UpdateLine(int pos, bool isRow, State state)
        {
            GetLine(pos, isRow).ForEach(e => e.State = state);
        }
        public void UpdateLine(int pos, int min, int length, bool isRow, State state)
        {
            GetLine(pos, isRow).GetRange(min, length).ForEach(e => e.State = state);
        }
        public void FillLine(int pos, bool isRow, State color)
        {
            GetLine(pos, isRow).ForEach(e => { if (e.State == State.Unknown) e.State = color; });
        }
        public int GetFirstWhiteIndex(int pos, bool isRow, bool isLeft, int refer)
        {
            List<int> whiteIndexList = GetLine(pos, isRow).Select((point, index) => (point, index)).Where(e => e.point.State == State.White && ((isLeft) ? e.index > refer : e.index < refer)).Select(e => e.index).ToList();
            return (isLeft) ? whiteIndexList.DefaultIfEmpty(-1).First() : whiteIndexList.DefaultIfEmpty(-1).Last();
        }

        public int GetFirstBlackIndex(int pos, bool isRow, bool isLeft, int refer)
        {
            List<int> blackIndexList = GetLine(pos, isRow).Select((point, index) => (point, index)).Where(e => e.point.State == State.Black && ((isLeft) ? e.index > refer : e.index < refer)).Select(e => e.index).ToList();
            return (isLeft) ? blackIndexList.DefaultIfEmpty(-1).First() : blackIndexList.DefaultIfEmpty(-1).Last();
        }
    }
}
