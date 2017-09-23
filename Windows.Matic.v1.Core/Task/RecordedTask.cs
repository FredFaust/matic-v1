namespace Windows.Matic.v1.Core.Task
{
    public class RecordedTask
    {
        private string _taskName;
        private InputChain _inputChain;

        public RecordedTask(string taskName, InputChain inputChain)
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
