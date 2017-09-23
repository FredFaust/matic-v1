using System.Windows.Forms;

namespace Windows.Matic.v1.Core.Task
{
    public class KeyboardEvent : InputEvent
    {
        private Keys _key;
        private KeyEventType _eventType;

        public KeyboardEvent(Keys k, KeyEventType eventType, int delay)
        {
            _key = k;
            _delay = delay;
            _eventType = eventType;
        }

        public Keys Key
        {
            get { return _key; }
        }

        public KeyEventType EventType
        {
            get { return _eventType; }
        }

        public override string ToString()
        {
            return "KeyboardEvent - " + _key + " " + _eventType;
        }
    }

    public enum KeyEventType
    {
        None,
        Down,
        Up
    }
}
