using System.Collections.Generic;
using System.Windows;
using Windows.Matic.v1.Core.Task;
using Windows.Matic.v1.Client.UserControls;
using Windows.Matic.v1.Client.Profile;
using System.IO;
using Newtonsoft.Json;
using System;
using Windows.Matic.v1.Client.Events;

namespace Windows.Matic.v1.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UserProfile _profile;
        private string _savedProfileLocation;

        private JsonSerializerSettings _customSerializerSettings;

        public MainWindow()
        {
            InitializeComponent();

            _customSerializerSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects
            };

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

        public void HandleNewTaskFinalized(object sender, TaskObjectEventArgs ea)
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
            tl.RaiseTaskDeleted += HandleTaskDeleted;
            contentControl.Content = tl;
        }

        public void HandleTaskDeleted(object sender, TaskObjectEventArgs ea)
        {
            _profile.DeleteComputerTask(ea.Task);
        }

        private void SaveUserProfile()
        {
            
            File.WriteAllText(_savedProfileLocation, JsonConvert.SerializeObject(_profile, _customSerializerSettings));
        }

        private UserProfile LoadUserProfile()
        {
            if (!File.Exists(_savedProfileLocation))
            {
                return new UserProfile("jffaust");
            }
            else
            {
                // TODO : Complex custom loader to populate Chain / List<InputEvent>
                string jsonProfile = File.ReadAllText(_savedProfileLocation);
                return JsonConvert.DeserializeObject<UserProfile>(jsonProfile, _customSerializerSettings);
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
