using Windows.Matic.v1.Core.Task;

namespace Windows.Matic.v1.Core.Player
{
    public class TaskPlayer
    {
        private InputSender _inputSender;

        public delegate void TaskExecutedCallback(TaskExecutionResults results);

        public TaskPlayer()
        {
            _inputSender = InputSender.Instance;
        }

        public void Execute(RecordedTask ut, TaskExecutedCallback callback)
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

            // Invoke the callback to notify the initiator class that the exeuction is completed
            callback?.Invoke(new TaskExecutionResults(true));
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
            _inputSender.SendMouseEvent(me.X, me.Y, me.MouseMessage);
        }
    }
}
