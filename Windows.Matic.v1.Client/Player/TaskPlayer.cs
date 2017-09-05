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
                if (ie.EventType == KeyEventType.Down)
                {
                    _inputSender.SendKeyDown(ie.Key);
                }
                else if (ie.EventType == KeyEventType.Up)
                {
                    _inputSender.SendKeyUp(ie.Key);
                }
                System.Threading.Thread.Sleep(ie.EventDelay);
            }
        }
    }
}
