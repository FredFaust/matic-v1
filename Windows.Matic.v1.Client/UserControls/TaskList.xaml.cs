using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Windows.Matic.v1.Player;
using Windows.Matic.v1.Task;

namespace Windows.Matic.v1.UserControls
{
    /// <summary>
    /// Interaction logic for MyTasks.xaml
    /// </summary>
    public partial class TaskList : UserControl
    {
        public Action NewTaskLinkClickedAction;

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
            NewTaskLinkClickedAction?.Invoke();
        }

        private void Button_Click_RunTask(object sender, RoutedEventArgs e)
        {
            UserTask ut = ((Button)sender).Tag as UserTask;
            MessageBox.Show(ut.TaskName);

            TaskPlayer tp = new TaskPlayer();
            tp.Execute(ut);
        }
    }
}
