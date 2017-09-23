namespace Windows.Matic.v1.Core.Task
{
    public class TaskExecutionResults
    {
        private bool _success;

        public TaskExecutionResults(bool success)
        {
            _success = success;
        }

        public bool Success
        {
            get { return _success; }
        }
    }
}
