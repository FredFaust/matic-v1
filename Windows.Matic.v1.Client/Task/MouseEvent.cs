namespace Windows.Matic.v1.Task
{
    public class MouseEvent : InputEvent
    {
        private int _posX;
        private int _posY;
        private int _flags;
        private int _mouseData;
        private MouseEventType _eventType;

        public MouseEvent(int x, int y, int flags, int mouseData, MouseEventType eventType, int delay)
        {
            _posX = x;
            _posY = y;
            _flags = flags;
            _delay = delay;
            _mouseData = mouseData;
            _eventType = eventType;
        }

        public int PosX
        {
            get { return _posX; }
        }

        public int PosY
        {
            get { return _posY; }
        }

        public int Flags
        {
            get { return _flags; }
        }

        public int MouseData
        {
            get { return _mouseData; }
        }

        public MouseEventType EventType
        {
            get { return _eventType; }
        }

        public override string ToString()
        {
            return "MouseEvent - " + _mouseData + " " + _eventType;
        }
    }

    public enum MouseEventType
    {
        None,
        Down,
        Up
    }
}
