using System;
using System.Windows.Forms;
using Windows.Matic.v1.Task;
using Windows.Matic.v1.Recorder.Session;

namespace Windows.Matic.v1.Recorder.Logger
{
    public class InputLogger
    {
        private KeyListener _keyListener;
        private RecordSession _currentSession;

        private Keys _reservedKeyPause = Keys.P;
        private Keys _reservedKeyResume = Keys.R;
        private Keys _reservedKeyDone = Keys.D;

        public Action RecordingDoneAction;

        public InputLogger()
        {
            _keyListener = KeyListener.Instance;
            _keyListener.ResetKeysetReadyIncovationList();
            _keyListener.RaiseKeysetReady += HandleKeysetReady;
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
                
        void HandleKeysetReady(object sender, KeysetReadyEventArgs ea)
        {
            InputCommand ic = new InputCommand(ea.Keyset);

            if (!IsReservedInputCommand(ic) && _currentSession.RecordingState == RecordingState.Active)
            {
                _currentSession.AddInputCommand(ic);
            }
            else
            {
                HandleReserverdInputCommand(ic);
            }
        }

        private void HandleReserverdInputCommand(InputCommand ic)
        {
            if (InputCommandEqualsReservedCommand(ic, _reservedKeyPause))
            {
                _currentSession.PauseRecording();
            }
            else if (InputCommandEqualsReservedCommand(ic, _reservedKeyResume))
            {
                _currentSession.ResumeRecording();
            }
            else if (InputCommandEqualsReservedCommand(ic, _reservedKeyDone))
            {
                StopLogging();
                RecordingDoneAction?.Invoke();
            }
        }

        public bool ReservedModifierKeysPressed(InputCommand ic)
        {
            bool ctrlKeyPresent = false;
            bool altKeyPresent = false;
            if (ic.Keyset.Contains(Keys.LControlKey) || ic.Keyset.Contains(Keys.RControlKey))
            {
                ctrlKeyPresent = true;
            }
            if (ic.Keyset.Contains(Keys.LMenu) || ic.Keyset.Contains(Keys.RMenu))
            {
                altKeyPresent = true;
            }

            return ctrlKeyPresent && altKeyPresent;
        }

        public bool InputCommandEqualsReservedCommand(InputCommand ic, Keys reservedKey)
        {
            if (!ReservedModifierKeysPressed(ic))
            {
                return false;
            }
            return ic.Keyset.Contains(reservedKey);
        }

        private bool IsReservedInputCommand(InputCommand ic)
        {
            return (InputCommandEqualsReservedCommand(ic, _reservedKeyPause) || InputCommandEqualsReservedCommand(ic, _reservedKeyResume) || InputCommandEqualsReservedCommand(ic, _reservedKeyDone));
        }

        public RecordSession CurrentSession
        {
            get { return _currentSession; }
        }
    }
}
