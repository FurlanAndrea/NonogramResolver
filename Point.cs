using System.Windows.Forms;

namespace NonogramResolver
{
    public class Point
    {
        readonly Button button;
        readonly Level level = new Level();
        public State State
        {
            get { return level.State; }
            set
            {
                level.State = value;
                button.BackColor = level.Color;
            }
        }
        public Point(Button b)
        {
            button = b;
        }
    }
}
