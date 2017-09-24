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

        public void Execute(ComputerTask task, TaskExecutedCallback callback)
        {
            foreach (InputEvent ie in task.InputChain.Chain)
            {
                System.Threading.Thread.Sleep(ie.DelayBeforeEvent);
                
                if (ie is KeyboardEvent)
                {
                    KeyboardEvent ke = ie as KeyboardEvent;
                    _inputSender.SendKeyboardEvent(ke.Key, (uint)ke.EventFlag);
                }
                else if (ie is MouseEvent)
                {
                    MouseEvent me = ie as MouseEvent;
                    _inputSender.SendMouseEvent(me.X, me.Y, me.MouseMessage);
                }
            }

            // Invoke the callback to notify the initiator class that the exeuction is completed
            callback?.Invoke(new TaskExecutionResults(true));
        }
    }
}
