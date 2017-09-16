using System;
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
            foreach (InputEvent ie in ut.InputChain.Chain)
            {
                System.Threading.Thread.Sleep(ie.DelayBeforeEvent);
                
                if (ie is KeyboardEvent)
                {
                    ExecuteKeyboardEvent(ie as KeyboardEvent);
                }
                else if (ie is MouseEvent)
                {
                    ExecuteMouseEvent(ie as MouseEvent);
                }
            }
        }

        private void ExecuteKeyboardEvent(KeyboardEvent ke)
        {
            if (ke.EventType == KeyEventType.Down)
            {
                _inputSender.SendKeyDownEvent(ke.Key);
            }
            else if (ke.EventType == KeyEventType.Up)
            {
                _inputSender.SendKeyUpEvent(ke.Key);
            }
        }

        private void ExecuteMouseEvent(MouseEvent me)
        {
            _inputSender.SendMouseEvent(me.PosX, me.PosY, me.Flags, me.MouseData);
        }
    }
}
