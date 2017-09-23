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

        public UserTask HardcodedTask;
        private List<UserTask> _userTasks;

        public TaskList(List<UserTask> userTasks)
        {
            InitializeComponent();

            _userTasks = userTasks;
            icUserTaskList.ItemsSource = _userTasks;
        }

        private void Button_Click_CreateNewTask(object sender, RoutedEventArgs e)
        {
            NavigateToNewTaskPage?.Invoke();
        }

        private void Button_Click_RunTask(object sender, RoutedEventArgs e)
        {
            UserTask task = ((Button)sender).Tag as UserTask;

            StartTaskExecution?.Invoke();
            MaticCoreFacade.PlayTask(task, TaskExecutionCompleted);
        }

        public void TaskExecutionCompleted(TaskExecutionResults results)
        {
            TaskExecutionDone?.Invoke();
        }
    }
}
