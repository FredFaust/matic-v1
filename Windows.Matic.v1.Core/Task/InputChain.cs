using System.Collections.Generic;

namespace Windows.Matic.v1.Core.Task
{
    public class InputChain
    {
        private List<InputEvent> _chain;

        public InputChain()
        {
            _chain = new List<InputEvent>();
        }

        public void AddInputEvent(InputEvent inputEvent)
        {
            _chain.Add(inputEvent);
        }

        public void AddInputEvents(List<InputEvent> inputEvents)
        {
            _chain.AddRange(inputEvents);
        }

        public List<InputEvent> Chain
        {
            get { return _chain;  }
        }
    }
}
