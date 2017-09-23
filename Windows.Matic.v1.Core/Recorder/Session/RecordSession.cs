using System.Collections.Generic;
using Windows.Matic.v1.Core.Task;

namespace Windows.Matic.v1.Core.Recorder.Session
{
    public class RecordSession
    {
        private InputChain _inputChain;
        private SessionState _sessionState;
        private RecordingState _recordingState;

        public RecordSession()
        {
            _inputChain = new InputChain();
            _sessionState = SessionState.Ready;
            _recordingState = RecordingState.Inactive;
        }

        public void AddInputEvent(KeyboardEvent inputEvent)
        {
            if (_recordingState == RecordingState.Active)
            {
                _inputChain.AddInputEvent(inputEvent);
            }
        }

        public void AddInputEvents(List<InputEvent> inputEvents)
        {
            if (_recordingState == RecordingState.Active)
            {
                _inputChain.AddInputEvents(inputEvents);
            }
        }

        public void StartRecording()
        {
            _sessionState = SessionState.InProgress;
            _recordingState = RecordingState.Active;
        }

        public void StopRecording()
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
