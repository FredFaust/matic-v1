using System.Windows.Forms;

namespace Windows.Matic.v1.Task
{
    public class InputEvent
    {
        private Keys _key;
        private KeyEventType _eventType;
        private int _eventDelay;

        public InputEvent(Keys k, KeyEventType ket, int delay)
        {
            _key = k;
            _eventType = ket;
            _eventDelay = delay;
        }

        public Keys Key
        {
            get { return _key; }
        }

        public KeyEventType EventType
        {
            get { return _eventType; }
        }

        public int EventDelay
        {
            get { return _eventDelay; }
        }
    }

    public enum KeyEventType
    {
        None,
        Down,
        Up
    }
}
