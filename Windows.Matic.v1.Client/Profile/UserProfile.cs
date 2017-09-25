using System.Collections.Generic;
using Windows.Matic.v1.Core.Task;

namespace Windows.Matic.v1.Client.Profile
{
    public class UserProfile
    {
        public UserProfile(string name)
        {
            Name = name;
            ComputerTasks = new List<ComputerTask>();
        }

        public void AddComputerTask(ComputerTask task)
        {
            ComputerTasks.Add(task);
        }

        public string Name { get; set; }

        public List<ComputerTask> ComputerTasks { get; set; }
    }
}
