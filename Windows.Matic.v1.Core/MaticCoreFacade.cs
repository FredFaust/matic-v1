using Windows.Matic.v1.Core.Player;
using Windows.Matic.v1.Core.Recorder;
using Windows.Matic.v1.Core.Task;
using static Windows.Matic.v1.Core.Player.TaskPlayer;
using static Windows.Matic.v1.Core.Recorder.InputRecorderMediator;

namespace Windows.Matic.v1.Core
{
    public static class MaticCoreFacade
    {
        public static void RecordTask(TaskRecordedCallback callback)
        {
            InputRecorderMediator inputRecorder = new InputRecorderMediator();
            inputRecorder.StartRecording(callback);
        }

        public static void PlayTask(ComputerTask task, TaskExecutedCallback callback)
        {
            TaskPlayer taskPlayer = new TaskPlayer();
            taskPlayer.Execute(task, callback);
        }
    }
}
