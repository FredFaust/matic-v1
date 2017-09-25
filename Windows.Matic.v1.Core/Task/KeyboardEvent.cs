using System.Windows.Forms;
using Windows.Matic.v1.Core.Common;

namespace Windows.Matic.v1.Core.Task
{
    public class KeyboardEvent : InputEvent
    {
        public KeyboardEvent(Keys k, KeyEventFlags eventFlag, int delay)
        {
            Key = k;
            EventFlag = eventFlag;
            DelayBeforeEvent = delay;
        }

        public Keys Key { get; set; }

        public KeyEventFlags EventFlag { get; set; }

        public override string ToString()
        {
            return "KeyboardEvent - " + Key + " " + EventFlag;
        }
    }
}
