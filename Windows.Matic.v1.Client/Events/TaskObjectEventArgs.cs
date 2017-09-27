using System;
using Windows.Matic.v1.Core.Task;

namespace Windows.Matic.v1.Client.Events
{
    public class TaskObjectEventArgs : EventArgs
    {
        public TaskObjectEventArgs(ComputerTask computerTask)
        {
            Task = computerTask;
        }

        public ComputerTask Task { get; set; }
    }
}
