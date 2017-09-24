namespace Windows.Matic.v1.Core.Task
{
    public class ComputerTask
    {
        private string _taskName;
        private InputChain _inputChain;

        public ComputerTask(string taskName, InputChain inputChain)
        {
            _taskName = taskName;
            _inputChain = inputChain;
        }

        public string TaskName
        {
            get { return _taskName; }
        }

        public InputChain InputChain
        {
            get { return _inputChain; }
        }
    }
}
