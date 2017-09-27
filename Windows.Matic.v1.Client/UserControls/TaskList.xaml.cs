using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Windows.Matic.v1.Core.Task;
using Windows.Matic.v1.Core.Player;
using Windows.Matic.v1.Core;
using Windows.Matic.v1.Client.Events;

namespace Windows.Matic.v1.Client.UserControls
{
    /// <summary>
    /// Interaction logic for MyTasks.xaml
    /// </summary>
    public partial class TaskList : UserControl
    {
        public Action StartTaskExecution;
        public Action TaskExecutionDone;
        public Action NavigateToNewTaskPage;

        public ComputerTask HardcodedTask;
        private List<ComputerTask> _computerTasks;

        public event EventHandler<TaskObjectEventArgs> RaiseTaskDeleted;

        public TaskList(List<ComputerTask> computerTasks)
        {
            InitializeComponent();

            _computerTasks = computerTasks;
            icComputerTaskList.ItemsSource = _computerTasks;
        }

        private void Button_Click_CreateNewTask(object sender, RoutedEventArgs e)
        {
            NavigateToNewTaskPage?.Invoke();
        }

        private void Button_Click_RunTask(object sender, RoutedEventArgs e)
        {
            ComputerTask task = ((Button)sender).Tag as ComputerTask;

            StartTaskExecution?.Invoke();
            MaticCoreFacade.PlayTask(task, TaskExecutionCompleted);
        }

        private void Button_Click_DeleteTask(object sender, RoutedEventArgs e)
        {
            ComputerTask task = ((Button)sender).Tag as ComputerTask;

            if (MessageBox.Show("Delete task forever?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _computerTasks.Remove(task);
                RaiseTaskDeleted?.Invoke(this, new TaskObjectEventArgs(task));
            }
        }

        public void TaskExecutionCompleted(TaskExecutionResults results)
        {
            TaskExecutionDone?.Invoke();
        }
    }
}
