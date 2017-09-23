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

        public RecordedTask HardcodedTask;
        private List<RecordedTask> _recordedTasks;

        public TaskList(List<RecordedTask> recordedTasks)
        {
            InitializeComponent();

            _recordedTasks = recordedTasks;
            icRecordedTaskList.ItemsSource = _recordedTasks;
        }

        private void Button_Click_CreateNewTask(object sender, RoutedEventArgs e)
        {
            NavigateToNewTaskPage?.Invoke();
        }

        private void Button_Click_RunTask(object sender, RoutedEventArgs e)
        {
            RecordedTask task = ((Button)sender).Tag as RecordedTask;

            StartTaskExecution?.Invoke();
            MaticCoreFacade.PlayTask(task, TaskExecutionCompleted);
        }

        public void TaskExecutionCompleted(TaskExecutionResults results)
        {
            TaskExecutionDone?.Invoke();
        }
    }
}
