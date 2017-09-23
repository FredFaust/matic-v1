using System.Collections.Generic;
using System.Windows;
using Windows.Matic.v1.Core.Task;
using Windows.Matic.v1.Client.UserControls;

namespace Windows.Matic.v1.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<RecordedTask> _recordedTasks;

        public MainWindow()
        {
            InitializeComponent();
            _recordedTasks = new List<RecordedTask>();

            SwitchViewToTaskList();
        }

        public void MinimizeClientWindow()
        {
            WindowState = WindowState.Minimized;
        }

        public void RestoreClientWindow()
        {
            WindowState = WindowState.Normal;
        }

        public void NavigateToNewTaskPage()
        {
            NewTask nt = new NewTask();
            nt.RecordingStartedAction = MinimizeClientWindow;
            nt.RaiseNewTaskFinalized += HandleNewTaskFinalized;
            contentControl.Content = nt;
        }

        public void HandleNewTaskFinalized(object sender, NewTaskFinalizedEventArgs ea)
        {
            _recordedTasks.Add(ea.Task);
            SwitchViewToTaskList();
            RestoreClientWindow();
        }

        private void SwitchViewToTaskList()
        {
            TaskList tl = new TaskList(_recordedTasks);
            tl.NavigateToNewTaskPage = NavigateToNewTaskPage;
            tl.StartTaskExecution = MinimizeClientWindow;
            tl.TaskExecutionDone = RestoreClientWindow;
            contentControl.Content = tl;
        }

        private void Button_Click_Tasks(object sender, RoutedEventArgs e)
        {
            SwitchViewToTaskList();
        }

        private void Button_Click_Schedule(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new ScheduleList();
        }
    }
}
