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
        private int _activeMouseButtons;
        private List<InputEvent> _inputEventsBuffer;

        private InputRecorderMediator _mediatorInstance;

        private Action ReservedCommandFoundAction;

        public InputHandler(InputRecorderMediator irm)
        {
            _mediatorInstance = irm;
            _activeMouseButtons = 0;
            _activeKeys = new List<Keys>();
            _inputEventsBuffer = new List<InputEvent>();
        }

        public int HandleKeyboardEventProc(int code, int wParam, ref KeyboardHookEventStruct lParam)
        {
            if (code >= 0)
            {
                int delay = GetDelaySinceLastEvent();
                
                Keys key = (Keys)lParam.vkCode;
                if ((wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN))
                {
                    HandleNewKeyboardEvent(new KeyboardEvent(key, KeyEventType.Down, delay));
                }
                else if ((wParam == WM_KEYUP || wParam == WM_SYSKEYUP))
                {
                    HandleNewKeyboardEvent(new KeyboardEvent(key, KeyEventType.Up, delay));
                }
            }

            /*if (PLAY_NICE)
            {
                return CallNextHookEx(hhook, code, wParam, ref lParam);
            }*/
            
            // we should not play nice if there are mean hooks on our system that unhook us
            return 0;
        }

        public int HandleMouseEventProc(int code, int wParam, ref MouseHookEventStruct lParam)
        {
            if (code >= 0 && wParam != WM_MOUSEMOVE)
            {
                int delay = GetDelaySinceLastEvent();

                if ((wParam == WM_LBUTTONDOWN || wParam == WM_RBUTTONDOWN))
                {
                    HandleNewMouseEvent(new MouseEvent(lParam.pt.x, lParam.pt.y, lParam.flags, lParam.mouseData, MouseEventType.Down, delay));
                }
                else if ((wParam == WM_LBUTTONUP || wParam == WM_RBUTTONUP))
                {
                    HandleNewMouseEvent(new MouseEvent(lParam.pt.x, lParam.pt.y, lParam.flags, lParam.mouseData, MouseEventType.Up, delay));
                }
                else
                {
                    HandleNewMouseEvent(new MouseEvent(lParam.pt.x, lParam.pt.y, lParam.flags, lParam.mouseData, MouseEventType.None, delay));
                }
            }

            return 0;
        }

        private int GetDelaySinceLastEvent()
        {
            int delaySinceLastEvent = 0;
            if (_firstEventHandled)
            {
                delaySinceLastEvent = (int)(DateTime.Now - _lastEventDateTime).TotalMilliseconds;
            }

            _firstEventHandled = true;
            _lastEventDateTime = DateTime.Now;

            return delaySinceLastEvent;
        }

        private void HandleNewKeyboardEvent(KeyboardEvent inputEvent)
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

        private void HandleNewMouseEvent(MouseEvent inputEvent)
        {
            if (!_reservedCommandInProgress)
            {
                _inputEventsBuffer.Add(inputEvent);
            }

            if (inputEvent.EventType == MouseEventType.Down && !_reservedCommandInProgress)
            {
                _activeMouseButtons += 1;
            }
            else if (inputEvent.EventType == MouseEventType.Up)
            {
                _activeMouseButtons -= 1;
                if (_activeMouseButtons == 0)
                {
                    SendInputEventsBufferToMediator();
                }
            }
            else
            {
                SendInputEventsBufferToMediator();
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
            KeyboardEvent lastKeyboardEvent = _inputEventsBuffer[iLastElement] as KeyboardEvent;

            if (lastKeyboardEvent != null)
            {
                if (lastKeyboardEvent.Key == reservedKey)
                {
                    List<int> indexesToRemove = new List<int>();
                    indexesToRemove.Add(iLastElement);

                    KeyboardEvent beforeLastKeyboardEvent = _inputEventsBuffer[iLastElement - 1] as KeyboardEvent;

                    if (beforeLastKeyboardEvent != null)
                    {
                        Keys beforeLastKey = beforeLastKeyboardEvent.Key;
                        if (beforeLastKey == Keys.LMenu || beforeLastKey == Keys.RMenu)
                        {
                            indexesToRemove.Add(iLastElement - 1);

                            bool isSuite = true;
                            for (int i = iLastElement - 2; i >= 0 && isSuite; i--)
                            {
                                KeyboardEvent indexedKeyboardEvent = _inputEventsBuffer[i] as KeyboardEvent;

                                if (indexedKeyboardEvent != null)
                                {
                                    if (indexedKeyboardEvent.Key == beforeLastKey)
                                    {
                                        indexesToRemove.Add(i);
                                    }
                                    else if (indexedKeyboardEvent.Key == Keys.LControlKey || indexedKeyboardEvent.Key == Keys.RControlKey)
                                    {
                                        indexesToRemove.Add(i);
                                    }
                                    else
                                    {
                                        isSuite = false;
                                    }
                                }
                            }
                        }
                        else if (beforeLastKey == Keys.LControlKey || beforeLastKey == Keys.RControlKey)
                        {
                            indexesToRemove.Add(iLastElement - 1);

                            bool isSuite = true;
                            for (int i = iLastElement - 2; i >= 0 && isSuite; i--)
                            {
                                KeyboardEvent indexedKeyboardEvent = _inputEventsBuffer[i] as KeyboardEvent;

                                if (indexedKeyboardEvent != null)
                                {
                                    if (indexedKeyboardEvent.Key == beforeLastKey)
                                    {
                                        indexesToRemove.Add(i);
                                    }
                                    else if (indexedKeyboardEvent.Key == Keys.LMenu || indexedKeyboardEvent.Key == Keys.RMenu)
                                    {
                                        indexesToRemove.Add(i);
                                    }
                                    else
                                    {
                                        isSuite = false;
                                    }
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
                        foreach (KeyboardEvent ie in _inputEventsBuffer.OfType<KeyboardEvent>())
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

                        foreach (Keys k in simulatedActiveKeys)
                        {
                            _inputEventsBuffer.Add(new KeyboardEvent(k, KeyEventType.Down, 50));
                        }
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Keyboard Message Identifiers
        /// </summary>
        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;
        const int WM_SYSKEYDOWN = 0x104;
        const int WM_SYSKEYUP = 0x105;


        /// <summary>
        ///  Mouse Message Identifiers
        /// </summary>
        const int WM_LBUTTONDOWN = 0x0201;
        const int WM_LBUTTONUP = 0x0202;
        const int WM_MOUSEMOVE = 0x0200;
        const int WM_MOUSEWHEEL = 0x020A;
        const int WM_RBUTTONDOWN = 0x0204;
        const int WM_RBUTTONUP = 0x0205;
    }
}
