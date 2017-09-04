using Windows.Matic.v1.Task;

namespace Windows.Matic.v1.Recorder.Session
{
    public class RecordSession
    {
        private SessionState _sessionState;
        private RecordingState _recordingState;
        private InputChain _inputChain;

        public RecordSession()
        {
            _inputChain = new InputChain();
            _sessionState = SessionState.Ready;
            _recordingState = RecordingState.Inactive;
        }

        public void AddInputCommand(InputCommand ic)
        {
            if (_recordingState == RecordingState.Active)
            {
                _inputChain.AddInputCommand(ic);
            }
        }

        public void StartRecording()
        {
            _sessionState = SessionState.InProgress;
            _recordingState = RecordingState.Active;
        }

        public void DoneRecording()
        {
            _recordingState = RecordingState.Inactive;
            _sessionState = SessionState.Finished;
        }

        public void PauseRecording()
        {
            _recordingState = RecordingState.Inactive;
        }

        public void ResumeRecording()
        {
            _recordingState = RecordingState.Active;
        }

        public SessionState SessionState
        {
            get { return _sessionState; }
        }

        public RecordingState RecordingState
        {
            get { return _recordingState; }
        }

        public InputChain InputChain
        {
            get { return _inputChain; }
        }
    }
}
