namespace Windows.Matic.v1.Core.Task
{
    public class ComputerTask
    {
        public ComputerTask(string taskName, InputChain inputChain)
        {
            TaskName = taskName;
            InputChain = inputChain;
        }

        public string TaskName { get; set; }

        public InputChain InputChain { get; set; }
    }
}
