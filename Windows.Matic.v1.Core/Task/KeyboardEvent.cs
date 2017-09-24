using System.Windows.Forms;
using Windows.Matic.v1.Core.Common;

namespace Windows.Matic.v1.Core.Task
{
    public class KeyboardEvent : InputEvent
    {
        private Keys _key;
        private KeyEventFlags _eventFlag;

        public KeyboardEvent(Keys k, KeyEventFlags eventFlag, int delay)
        {
            _key = k;
            _delay = delay;
            _eventFlag = eventFlag;
        }

        public Keys Key
        {
            get { return _key; }
        }

        public KeyEventFlags EventFlag
        {
            get { return _eventFlag; }
        }

        public override string ToString()
        {
            return "KeyboardEvent - " + _key + " " + _eventFlag;
        }
    }
}
