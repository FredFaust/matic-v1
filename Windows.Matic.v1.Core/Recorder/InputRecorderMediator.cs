using System;
using System.Collections.Generic;
using Windows.Matic.v1.Core.Recorder.Handler;
using Windows.Matic.v1.Core.Recorder.Listener;
using Windows.Matic.v1.Core.Recorder.Session;
using Windows.Matic.v1.Core.Task;

namespace Windows.Matic.v1.Core.Recorder
{
    /// <summary>
    /// Serves as a Mediator between the different components required to record user input
    /// </summary>
    public class InputRecorderMediator
    {
        private InputListener _inputListener;
        private InputHandler _inputHandler;
        private RecordSession _currentSession;

        private TaskRecordedCallback _taskRecordedCallback;
        public delegate void TaskRecordedCallback(InputChain ic);

        public InputRecorderMediator()
        {
            _currentSession = new RecordSession();
            _inputHandler = new InputHandler(this);
            _inputListener = InputListener.Instance;
        }

        public void ReceiveInputEventsBuffer(List<InputEvent> buffer)
        {
            if (_currentSession.RecordingState == RecordingState.Active)
            {
                _currentSession.AddInputEvents(buffer);
            }
        }

        public void StartRecording(TaskRecordedCallback trc)
        {
            _taskRecordedCallback = trc;
            _currentSession.StartRecording();
            _inputListener.StartListening(_inputHandler.HandleKeyboardEventProc, _inputHandler.HandleMouseEventProc);
        }

        public void StopRecording()
        {
            _currentSession.StopRecording();
            _inputListener.StopListening();

            // Invoke the callback to notify the initiator class that the recording is completed
            _taskRecordedCallback?.Invoke(_currentSession.InputChain);
        }

        public void PauseRecording()
        {
            _currentSession.PauseRecording();
        }

        public void ResumeRecording()
        {
            _currentSession.ResumeRecording();
        }

        public RecordSession CurrentSession
        {
            get { return _currentSession; }
        }
    }
}
