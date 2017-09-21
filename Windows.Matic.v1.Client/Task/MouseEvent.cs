using Windows.Matic.v1.Common;

namespace Windows.Matic.v1.Task
{
    public class MouseEvent : InputEvent
    {
        private int _posX;
        private int _posY;
        private int _mouseMessage;

        public MouseEvent(int x, int y, int mouseMessage, int delay)
        {
            _posX = x;
            _posY = y;
            _delay = delay;
            _mouseMessage = mouseMessage;
        }

        public int X
        {
            get { return _posX; }
        }

        public int Y
        {
            get { return _posY; }
        }

        public int MouseMessage
        {
            get { return _mouseMessage; }
        }

        public override string ToString()
        {
            return "MouseEvent - (" + _posX + "," + _posY + ") " + _mouseMessage;
        }
    }
}
