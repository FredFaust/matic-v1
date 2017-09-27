using System;
using System.Windows;
using System.Windows.Controls;
using Windows.Matic.v1.Client.Events;
using Windows.Matic.v1.Core;
using Windows.Matic.v1.Core.Task;

namespace Windows.Matic.v1.Client.UserControls
{
    /// <summary>
    /// Interaction logic for RecordTask.xaml
    /// </summary>
    public partial class NewTask : UserControl
    {
        public Action RecordingStartedAction;
        public event EventHandler<TaskObjectEventArgs> RaiseNewTaskFinalized;

        public NewTask()
        {
            InitializeComponent();

        }

        private void Button_Click_StartRecording(object sender, RoutedEventArgs e)
        {
            btn_start_recording.IsEnabled = false;
            RecordingStartedAction?.Invoke();
            MaticCoreFacade.RecordTask(TaskRecordingCompleted);
        }

        public void TaskRecordingCompleted(InputChain ic)
        {
            ComputerTask task = new ComputerTask(txtTaskName.Text, ic);
            RaiseNewTaskFinalized?.Invoke(this, new TaskObjectEventArgs(task));
        }
    }

    
}
