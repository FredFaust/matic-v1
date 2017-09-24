using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Matic.v1.Core.Task;

namespace Windows.Matic.v1.Client.Profile
{
    public class UserProfile
    {
        private string _name;
        private List<ComputerTask> _computerTasks;

        public UserProfile(string name)
        {
            _name = name;
            _computerTasks = new List<ComputerTask>();
        }

        public void AddComputerTask(ComputerTask task)
        {
            _computerTasks.Add(task);
        }

        public string Name
        {
            get { return _name; }
        }
    }
}
