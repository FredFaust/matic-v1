using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windows.Matic.v1.Task
{
    public class InputEvent
    {
        protected int _delay;
        public int DelayBeforeEvent { get { return _delay; } }
    }
}
