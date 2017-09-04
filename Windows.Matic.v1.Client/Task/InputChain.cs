using System.Collections.Generic;

namespace Windows.Matic.v1.Task
{
    public class InputChain
    {
        private List<InputCommand> _chain; 

        public InputChain()
        {
            _chain = new List<InputCommand>();
        }

        public void AddInputCommand(InputCommand ic)
        {
            _chain.Add(ic);
        }

        public List<InputCommand> Chain
        {
            get { return _chain;  }
        }
    }
}
