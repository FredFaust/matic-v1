using System.Collections.Generic;

namespace Windows.Matic.v1.Core.Task
{
    public class InputChain
    {
        public InputChain()
        {
            Chain = new List<InputEvent>();
        }

        public void AddInputEvent(InputEvent inputEvent)
        {
            Chain.Add(inputEvent);
        }

        public void AddInputEvents(List<InputEvent> inputEvents)
        {
            Chain.AddRange(inputEvents);
        }

        public List<InputEvent> Chain { get; set; }
    }
}
