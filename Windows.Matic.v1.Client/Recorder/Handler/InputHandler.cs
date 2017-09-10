using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Matic.v1.Recorder.Session;
using Windows.Matic.v1.Task;
using static Windows.Matic.v1.Recorder.Listener.InputListener;

namespace Windows.Matic.v1.Recorder.Handler
{
    public class InputHandler
    {
        private DateTime _lastEventDateTime;
        private bool _firstEventHandled = false;

        private bool _reservedCommandInProgress = false;

        private List<Keys> _activeKeys;
        private List<InputEvent> _inputEventsBuffer;

        private InputRecorderMediator _mediatorInstance;

        private Action ReservedCommandFoundAction;

        public InputHandler(InputRecorderMediator irm)
        {
            _mediatorInstance = irm;
            _activeKeys = new List<Keys>();
            _inputEventsBuffer = new List<InputEvent>();
        }

        public int HandleKeyboardEventProc(int code, int wParam, ref KeyboardHookEventStruct lParam)
        {
            if (code >= 0)
            {
                int delaySinceLastEvent = 0;
                if (_firstEventHandled)
                {
                    delaySinceLastEvent = (int)(DateTime.Now - _lastEventDateTime).TotalMilliseconds;   
                }

                _firstEventHandled = true;
                _lastEventDateTime = DateTime.Now;
                
                Keys key = (Keys)lParam.vkCode;
                if ((wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN))
                {
                    HandleNewInputEvent(new InputEvent(key, KeyEventType.Down, delaySinceLastEvent));
                }
                else if ((wParam == WM_KEYUP || wParam == WM_SYSKEYUP))
                {
                    HandleNewInputEvent(new InputEvent(key, KeyEventType.Up, delaySinceLastEvent));
                }
            }

            /*if (PLAY_NICE)
            {
                return CallNextHookEx(hhook, code, wParam, ref lParam);
            }
            else
            {*/
            // we should not play nice if there are mean hooks on our system that unhook us
            return 0;
            //}
        }

        private void HandleNewInputEvent(InputEvent inputEvent)
        {
            if (!_reservedCommandInProgress)
            {
                _inputEventsBuffer.Add(inputEvent);
            }

            if (inputEvent.EventType == KeyEventType.Down && !_reservedCommandInProgress && !_activeKeys.Contains(inputEvent.Key))
            {
                _activeKeys.Add(inputEvent.Key);
                if (ReservedCommandFoundInActiveKeys())
                {
                    _reservedCommandInProgress = true;
                    ReservedCommandFoundAction?.Invoke();
                }
            }
            else if (inputEvent.EventType == KeyEventType.Up && _activeKeys.Contains(inputEvent.Key))
            {
                _activeKeys.Remove(inputEvent.Key);
                if (!_activeKeys.Any())
                {
                    _reservedCommandInProgress = false;
                    SendInputEventsBufferToMediator();
                }
            }
        }

        private void SendInputEventsBufferToMediator()
        {
            if (_inputEventsBuffer.Any())
            {
                _mediatorInstance.ReceiveInputEventsBuffer(_inputEventsBuffer);
                _inputEventsBuffer = new List<InputEvent>();
            }
        }

        private void HandleReservedCommandPause()
        {
            ReservedCommandFoundAction = null;
            RemoveReservedCommandEventsFromBuffer(_reservedKeyPause);
            SendInputEventsBufferToMediator();
            _mediatorInstance.PauseRecording();
        }

        private void HandleReservedCommandResume()
        {
            ReservedCommandFoundAction = null;
            RemoveReservedCommandEventsFromBuffer(_reservedKeyResume);
            SendInputEventsBufferToMediator();
            _mediatorInstance.ResumeRecording();
        }

        private void HandleReservedCommandDone()
        {
            ReservedCommandFoundAction = null;
            RemoveReservedCommandEventsFromBuffer(_reservedKeyDone);
            SendInputEventsBufferToMediator();
            _mediatorInstance.StopRecording();
        }

        #region Reserved Commands

        private Keys _reservedKeyPause = Keys.P;
        private Keys _reservedKeyResume = Keys.R;
        private Keys _reservedKeyDone = Keys.D;

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
            else if (CurrentActiveKeysIsReservedCommand(_reservedKeyDone))
            {
                reservedCommandFound = true;
                ReservedCommandFoundAction = HandleReservedCommandDone;
            }

            return reservedCommandFound;
        }

        private bool CurrentActiveKeysIsReservedCommand(Keys reservedKey)
        {
            if (!ReservedModifierKeysPressed())
            {
                return false;
            }
            return _activeKeys.Contains(reservedKey) && _activeKeys.Count == 3;
        }

        private bool ReservedModifierKeysPressed()
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

        private void RemoveReservedCommandEventsFromBuffer(Keys reservedKey)
        {
            int iLastElement = _inputEventsBuffer.Count - 1;
            if (_inputEventsBuffer[iLastElement].Key == reservedKey)
            {
                List<int> indexesToRemove = new List<int>();
                indexesToRemove.Add(iLastElement);

                Keys beforeLastKey = _inputEventsBuffer[iLastElement - 1].Key;
                if (beforeLastKey == Keys.LMenu || beforeLastKey == Keys.RMenu)
                {
                    indexesToRemove.Add(iLastElement - 1);

                    bool isSuite = true;
                    for (int i = iLastElement - 2; i >= 0 && isSuite; i--)
                    {
                        if (_inputEventsBuffer[i].Key == beforeLastKey)
                        {
                            indexesToRemove.Add(i);
                        }
                        else if (_inputEventsBuffer[i].Key == Keys.LControlKey || _inputEventsBuffer[i].Key == Keys.RControlKey)
                        {
                            indexesToRemove.Add(i);
                        }
                        else
                        {
                            isSuite = false;
                        }
                    }
                }
                else if (beforeLastKey == Keys.LControlKey || beforeLastKey == Keys.RControlKey)
                {
                    indexesToRemove.Add(iLastElement - 1);

                    bool isSuite = true;
                    for (int i = iLastElement - 2; i >= 0 && isSuite; i--)
                    {
                        if (_inputEventsBuffer[i].Key == beforeLastKey)
                        {
                            indexesToRemove.Add(i);
                        }
                        else if (_inputEventsBuffer[i].Key == Keys.LMenu || _inputEventsBuffer[i].Key == Keys.RMenu)
                        {
                            indexesToRemove.Add(i);
                        }
                        else
                        {
                            isSuite = false;
                        }
                    }
                }

                foreach (int i in indexesToRemove)
                {
                    _inputEventsBuffer.RemoveAt(i);
                }

                // Repass through the buffer and make sure no keys are being hold down
                // because of the use case where a user chains command with Ctrl and ends
                // up with a reserved command
                List<Keys> simulatedActiveKeys = new List<Keys>();
                foreach(InputEvent ie in _inputEventsBuffer)
                {
                    if (ie.EventType == KeyEventType.Down && !simulatedActiveKeys.Contains(ie.Key))
                    {
                        simulatedActiveKeys.Add(ie.Key);
                    }
                    else if (ie.EventType == KeyEventType.Up && simulatedActiveKeys.Contains(ie.Key))
                    {
                        simulatedActiveKeys.Remove(ie.Key);
                    }
                }

                foreach(Keys k in simulatedActiveKeys)
                {
                    _inputEventsBuffer.Add(new InputEvent(k, KeyEventType.Down, 50));
                }
            }
        }
        #endregion

        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;
        const int WM_SYSKEYDOWN = 0x104;
        const int WM_SYSKEYUP = 0x105;
    }
}
