using System.Collections.Generic;

namespace Windows.Matic.v1.Task
{
    public class InputChain
    {
        private List<InputEvent> _chain;

        public InputChain()
        {
            _chain = new List<InputEvent>();
        }

        public void AddInputEvent(InputEvent ie)
        {
            _chain.Add(ie);
        }

        public List<InputEvent> Chain
        {
            get { return _chain;  }
        }
    }
}
