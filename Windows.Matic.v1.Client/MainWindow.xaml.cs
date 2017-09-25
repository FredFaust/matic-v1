using System.Collections.Generic;
using System.Windows;
using Windows.Matic.v1.Core.Task;
using Windows.Matic.v1.Client.UserControls;
using Windows.Matic.v1.Client.Profile;
using System.IO;
using Newtonsoft.Json;
using System;

namespace Windows.Matic.v1.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UserProfile _profile;
        private string _savedProfileLocation;

        public MainWindow()
        {
            InitializeComponent();
            _savedProfileLocation = System.Windows.Forms.Application.UserAppDataPath + "\\profile.json";
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

            _profile = LoadUserProfile();
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
            _profile.AddComputerTask(ea.Task);
            SwitchViewToTaskList();
            RestoreClientWindow();
        }

        private void SwitchViewToTaskList()
        {
            TaskList tl = new TaskList(_profile.ComputerTasks);
            tl.NavigateToNewTaskPage = NavigateToNewTaskPage;
            tl.StartTaskExecution = MinimizeClientWindow;
            tl.TaskExecutionDone = RestoreClientWindow;
            contentControl.Content = tl;
        }

        private void SaveUserProfile()
        {
            File.WriteAllText(_savedProfileLocation, JsonConvert.SerializeObject(_profile));
        }

        private UserProfile LoadUserProfile()
        {
            if (!File.Exists(_savedProfileLocation))
            {
                return new UserProfile("jffaust");
            }
            else
            {
                string jsonProfile = File.ReadAllText(_savedProfileLocation);
                return JsonConvert.DeserializeObject<UserProfile>(jsonProfile);
            }
        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            SaveUserProfile();
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
