using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Windows.Matic.v1.Task;

namespace Windows.Matic.v1.Player
{
    class TaskPlayer
    {
        private InputSender _inputSender;

        public TaskPlayer()
        {
            _inputSender = InputSender.Instance;
        }

        public void Execute(UserTask ut)
        {
            foreach (InputCommand ic in ut.InputChain.Chain)
            {
                foreach (Keys k in ic.Keyset)
                {
                    _inputSender.SendKeyPress(k);
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }
    }
}
