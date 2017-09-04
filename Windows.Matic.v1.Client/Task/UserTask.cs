namespace Windows.Matic.v1.Task
{
    public class UserTask
    {
        private string _taskName;
        private InputChain _inputChain;

        public UserTask(string taskName, InputChain inputChain)
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
