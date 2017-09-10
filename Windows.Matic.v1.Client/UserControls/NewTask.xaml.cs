using System;
using System.Windows;
using System.Windows.Controls;
using Windows.Matic.v1.Recorder;
using Windows.Matic.v1.Task;

namespace Windows.Matic.v1.UserControls
{
    public class NewTaskFinalizedEventArgs : EventArgs
    {
        private UserTask _userTask;

        public NewTaskFinalizedEventArgs(UserTask userTask)
        {
            _userTask = userTask;
        }

        public UserTask UserTask
        {
            get { return _userTask; }
        }
    }

    /// <summary>
    /// Interaction logic for RecordTask.xaml
    /// </summary>
    public partial class NewTask : UserControl
    {
        public Action RecordingStartedAction;
        public event EventHandler<NewTaskFinalizedEventArgs> RaiseNewTaskFinalized;

        private InputRecorderMediator _inputRecorder;

        public NewTask()
        {
            InitializeComponent();

            _inputRecorder = new InputRecorderMediator();
            _inputRecorder.RecordingDoneAction = FinalizeUserTask;
        }

        private void Button_Click_StartRecording(object sender, RoutedEventArgs e)
        {
            btn_start_recording.IsEnabled = false;
            RecordingStartedAction?.Invoke();
            _inputRecorder.StartRecording();
        }

        public void FinalizeUserTask()
        {
            UserTask ut = new UserTask(txtTaskName.Text, _inputRecorder.CurrentSession.InputChain);
            OnRaiseNewTaskFinalized(ut);
        }

        private void OnRaiseNewTaskFinalized(UserTask ut)
        {
            EventHandler<NewTaskFinalizedEventArgs> handler = RaiseNewTaskFinalized;

            if (handler != null)
            {
                NewTaskFinalizedEventArgs ea = new NewTaskFinalizedEventArgs(ut);
                handler(this, ea);
            }
        }
    }
}
