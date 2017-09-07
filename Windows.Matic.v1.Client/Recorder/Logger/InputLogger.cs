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
        private List<InputEvent> _inputEventsBuffer;
        private bool _canAcceptInput = true;

        private KeyListener _keyListener;
        private RecordSession _currentSession;
        
        public Action RecordingDoneAction;
        public Action ReservedCommandFoundAction;

        private Keys _reservedKeyPause = Keys.P;
        private Keys _reservedKeyResume = Keys.R;
        private Keys _reservedKeyFinish = Keys.D;

        public InputLogger()
        {
            _activeKeys = new List<Keys>();
            _inputEventsBuffer = new List<InputEvent>();

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
        }       

        private void UpdateActiveKeys(InputEvent ie)
        {
            if (_canAcceptInput)
            {
                _inputEventsBuffer.Add(ie);
            }

            if (ie.EventType == KeyEventType.Down)
            {
                if (!_activeKeys.Contains(ie.Key))
                {
                    _activeKeys.Add(ie.Key);
                    if (ReservedCommandFoundInActiveKeys())
                    {
                        _canAcceptInput = false;
                        ReservedCommandFoundAction?.Invoke();
                    }
                }
            }
            else if (ie.EventType == KeyEventType.Up)
            {
                if (_activeKeys.Contains(ie.Key))
                {
                    _activeKeys.Remove(ie.Key);
                }
            }
            
            if (!_activeKeys.Any())
            {
                _canAcceptInput = true;
                if (_inputEventsBuffer.Any())
                {
                    _currentSession.AddInputEvents(_inputEventsBuffer);
                    _inputEventsBuffer = new List<InputEvent>();
                }
            }
        }

        private bool ReservedCommandFoundInActiveKeys()
        {
            bool reservedCommandFound = false;

            if (CurrentActiveKeysIsReservedCommand(_reservedKeyPause))
            {
                reservedCommandFound = true;
                ReservedCommandFoundAction = HandleReservedCommandPause;
            } 
            else if (CurrentActiveKeysIsReservedCommand(_reservedKeyResume))
            {
                reservedCommandFound = true;
                ReservedCommandFoundAction = HandleReservedCommandResume;
            } 
            else if (CurrentActiveKeysIsReservedCommand(_reservedKeyFinish))
            {
                reservedCommandFound = true;
                ReservedCommandFoundAction = HandleReservedCommandFinish;
            }

            return reservedCommandFound;
        }

        private void HandleReservedCommandPause()
        {
            ReservedCommandFoundAction = null;
            _inputEventsBuffer = new List<InputEvent>();
            _currentSession.PauseRecording();
        }

        private void HandleReservedCommandResume()
        {
            ReservedCommandFoundAction = null;
            _inputEventsBuffer = new List<InputEvent>();
            _currentSession.ResumeRecording();
        }

        private void HandleReservedCommandFinish()
        {
            ReservedCommandFoundAction = null;
            _inputEventsBuffer = new List<InputEvent>();
            StopLogging();
            RecordingDoneAction?.Invoke();
        }

        private bool CurrentActiveKeysIsReservedCommand(Keys reservedKey)
        {
            if (!ReservedModifierKeysPressed())
            {
                return false;
            }
            return _activeKeys.Contains(reservedKey) && _activeKeys.Count == 3;
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

        public RecordSession CurrentSession
        {
            get { return _currentSession; }
        }
    }
}
