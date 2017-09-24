using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Windows.Matic.v1.Core.Task;
using Windows.Matic.v1.Core.Player;
using Windows.Matic.v1.Core;

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

        public void TaskExecutionCompleted(TaskExecutionResults results)
        {
            TaskExecutionDone?.Invoke();
        }
    }
}
