using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace NonogramResolver
{
    class DirectionsWrapper
    {
        List<Line> lines = new List<Line>();
        public Dimensions Dimension { get; set; }

        public int CreateLines(List<string> h, List<string> v)
        {
            List<Line> horizontal = h.Select((e, i) => new Line(e, i, true)).ToList();
            List<Line> vertical = v.Select((e, i) => new Line(e, i, false)).ToList();
            Dimension = new Dimensions(horizontal.Count, vertical.Count, horizontal.Max(e => e.Band.Count), vertical.Max(e => e.Band.Count));
            lines.AddRange(horizontal);
            lines.AddRange(vertical);
            return (horizontal.Sum(e => e.Band.Sum()) == vertical.Sum(e => e.Band.Sum())) ? horizontal.Sum(e => e.Band.Sum()) : -1;
        }

        public void CreateLinesButtons(Form form)
        {
            lines.ForEach(e => e.CreateButton(Dimension, form));
        }

        public void ResizeForm(Form form)
        {
            form.Location = new System.Drawing.Point(0, 0);
            form.Size = new Size(27 + (Consts.GetSize(Dimension) + 1) * (Dimension.VerticalLength + Dimension.HorizontalMax), 50 + (Consts.GetSize(Dimension) + 1) * (Dimension.HorizontalLength + Dimension.VerticalMax));
        }

        public void Fill(Grid grid)
        {
            lines = lines.Where(e => !e.RemoveObvious(Dimension, grid)).ToList();
            lines.ForEach(e => e.FillObviousBlack(Dimension, grid));
            lines = lines.Where(e => !e.FillBorders(grid, true)).ToList();
            lines = lines.Where(e => !e.FillBorders(grid, false)).ToList();
        }

        public void PartialFill(Grid grid)
        {
            lines.ForEach(e => e.PartialFill(grid, true));
            lines.ForEach(e => e.PartialFill(grid, false));
        }

        public void Attempt(Grid grid)
        {
            lines.Sort(delegate (Line x, Line y)
            {
                int unknownX = grid.GetUnknowns(x.pos, x.isRow);
                int unknownY = grid.GetUnknowns(y.pos, y.isRow);
                return Convert.ToInt32(unknownX > unknownY) - Convert.ToInt32(unknownX < unknownY);
            });
            foreach (Line line in lines) if (line.AttemptOne(grid)) break;
        }

        public bool CheckConsistency()
        {
            return lines.Select(e => e.CheckOverflow(Dimension)).Sum(e => e ? 0 : 1) == 0;
        }
    }
}
