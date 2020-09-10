using System;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;

namespace NonogramResolver
{
    class Nonogram
    {
        readonly DirectionsWrapper dirWrapper = new DirectionsWrapper();
        readonly Grid grid = new Grid();
        int blackTiles = 0;
        public void CreateNonogram(string text, Form form)
        {
            try
            {
                string[] directions = text.Split(Consts.dirSeparator, StringSplitOptions.None);
                List<string> horizontal = directions[0].Split(Consts.lineSeparator, StringSplitOptions.None).ToList();
                List<string> vertical = directions[1].Split(Consts.lineSeparator, StringSplitOptions.None).ToList();
                horizontal.RemoveRange(0, 2);
                vertical.RemoveAt(vertical.Count - 1);
                blackTiles = dirWrapper.CreateLines(horizontal, vertical);
                if (dirWrapper.CheckConsistency() && blackTiles != -1)
                {
                    dirWrapper.CreateLinesButtons(form);
                    grid.CreateGrid(form, dirWrapper.Dimension);
                    dirWrapper.ResizeForm(form);
                }
                else
                {
                    MessageBox.Show("Inconsistent Nonogram");
                    Application.Exit();
                }
            }
            catch
            {
                MessageBox.Show("An error occurred while reading the file.");
                Application.Exit();
            }
        }

        public void Solve(Form form)
        {
            int tempBlack = -1;
            int tempWhite = -1;
            while (tempBlack != grid.GetBlacks() || tempWhite != grid.GetWhites())
            {
                while (tempBlack != grid.GetBlacks() || tempWhite != grid.GetWhites())
                {
                    while (tempBlack != grid.GetBlacks() || tempWhite != grid.GetWhites())
                    {
                        tempBlack = grid.GetBlacks();
                        tempWhite = grid.GetWhites();
                        dirWrapper.Fill(grid);
                    }
                    dirWrapper.PartialFill(grid);
                }
                form.Refresh();
                dirWrapper.Attempt(grid);
            }
        }
    }
}