namespace Windows.Matic.v1.Core.Task
{
    public class MouseEvent : InputEvent
    {
        public MouseEvent(int x, int y, int mouseMessage, int delay)
        {
            X = x;
            Y = y;
            DelayBeforeEvent = delay;
            MouseMessage = mouseMessage;
        }

        public int X { get; set; }

        public int Y { get; set; }

        public int MouseMessage { get; set; }

        public override string ToString()
        {
            return "MouseEvent - (" + X + "," + Y + ") " + MouseMessage;
        }
    }
}
