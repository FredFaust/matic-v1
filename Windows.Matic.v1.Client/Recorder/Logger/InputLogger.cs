using System;
using System.Windows.Forms;
using Windows.Matic.v1.Task;
using Windows.Matic.v1.Recorder.Session;
using System.Collections.Generic;
using System.Linq;

namespace Windows.Matic.v1.Recorder.Logger
{
    public class InputLogger
    {
        private List<Keys> _activeKeys;
        private KeyListener _keyListener;
        private RecordSession _currentSession;
        
        public Action RecordingDoneAction;

        private Keys _reservedKeyPause = Keys.P;
        private Keys _reservedKeyResume = Keys.R;
        private Keys _reservedKeyFinish = Keys.D;

        public InputLogger()
        {
            _activeKeys = new List<Keys>();
            _keyListener = KeyListener.Instance;
            _keyListener.ResetInputEventReadyIncovationList();
            _keyListener.RaiseInputEventReady += HandleInputEventReady;
            _currentSession = new RecordSession();
        }

        public void StartLogging()
        {
            _keyListener.StartLogging();
            _currentSession.StartRecording();
        }

        public void StopLogging()
        {
            _keyListener.StopLogging();
            _currentSession.DoneRecording();
        }
                
        void HandleInputEventReady(object sender, InputEventReadyEventArgs ea)
        {
            InputEvent ie = ea.InputEvent;

            UpdateActiveKeys(ie);

            if (!CurrentActiveKeysAreReservedCommands() && _currentSession.RecordingState == RecordingState.Active)
            {
                _currentSession.AddInputEvent(ie);
            }
            else
            {
                HandleReserverdInputCommand();
            }
        }

        private void HandleReserverdInputCommand()
        {
            if (CurrentActiveKeysIsReservedCommand(_reservedKeyPause))
            {
                _currentSession.PauseRecording();
            }
            else if (CurrentActiveKeysIsReservedCommand(_reservedKeyResume))
            {
                _currentSession.ResumeRecording();
            }
            else if (CurrentActiveKeysIsReservedCommand(_reservedKeyFinish))
            {
                StopLogging();
                RecordingDoneAction?.Invoke();
            }
        }

        private bool CurrentActiveKeysAreReservedCommands()
        {
            return (CurrentActiveKeysIsReservedCommand(_reservedKeyPause) || CurrentActiveKeysIsReservedCommand(_reservedKeyResume) || CurrentActiveKeysIsReservedCommand(_reservedKeyFinish));
        }

        private bool CurrentActiveKeysIsReservedCommand(Keys reservedKey)
        {
            if (!ReservedModifierKeysPressed())
            {
                return false;
            }
            return _activeKeys.Contains(reservedKey);
        }

        public bool ReservedModifierKeysPressed()
        {
            bool ctrlKeyPresent = false;
            bool altKeyPresent = false;
            if (_activeKeys.Contains(Keys.LControlKey) || _activeKeys.Contains(Keys.RControlKey))
            {
                ctrlKeyPresent = true;
            }
            if (_activeKeys.Contains(Keys.LMenu) || _activeKeys.Contains(Keys.RMenu))
            {
                altKeyPresent = true;
            }

            return ctrlKeyPresent && altKeyPresent;
        }

        private void UpdateActiveKeys(InputEvent ie)
        {
            if (ie.EventType == KeyEventType.Down)
            {
                if (!_activeKeys.Contains(ie.Key))
                {
                    _activeKeys.Add(ie.Key);
                }
            }
            else if (ie.EventType == KeyEventType.Up)
            {
                if (_activeKeys.Contains(ie.Key))
                {
                    _activeKeys.Remove(ie.Key);
                }
            }
        }

        public RecordSession CurrentSession
        {
            get { return _currentSession; }
        }
    }
}
