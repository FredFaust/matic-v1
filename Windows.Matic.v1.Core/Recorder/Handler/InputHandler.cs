using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Windows.Matic.v1.Core.Common;
using Windows.Matic.v1.Core.Recorder.ReservedCommands;
using Windows.Matic.v1.Core.Task;

namespace Windows.Matic.v1.Core.Recorder.Handler
{
    public class InputHandler
    {
        private DateTime _lastEventDateTime;
        private bool _firstEventHandled = false;

        private bool _reservedCommandInProgress = false;
        private ReservedCommandChecker _reservedCommandChecker;

        private List<Keys> _activeKeys;
        private int _activeMouseButtons;
        private List<InputEvent> _inputEventsBuffer;

        private InputRecorderMediator _mediatorInstance;

        private string _logFileFullName;

        public InputHandler(InputRecorderMediator irm)
        {
            _mediatorInstance = irm;
            _activeMouseButtons = 0;
            _activeKeys = new List<Keys>();

            _inputEventsBuffer = new List<InputEvent>();
            _reservedCommandChecker = new ReservedCommandChecker();

            _logFileFullName = $@"{Directory.GetCurrentDirectory()}\RecordSessionLogs_{DateTime.Now.ToString("yyyyMMdd_hhmmss")}.txt";
            string text = "Logs for the latest record session : \n";
            File.WriteAllText(_logFileFullName, text);
        }

        public int HandleKeyboardEventProc(int code, int wParam, ref KeyboardHookEventStruct lParam)
        {
            if (code >= 0)
            {
                int delay = GetDelaySinceLastEvent();
                
                Keys key = (Keys)lParam.vkCode;
                if ((wParam == KeyboardMessages.WM_KEYDOWN || wParam == KeyboardMessages.WM_SYSKEYDOWN))
                {
                    HandleNewInputEvent(new KeyboardEvent(key, KeyEventFlags.KeyDown, delay));
                }
                else if ((wParam == KeyboardMessages.WM_KEYUP || wParam == KeyboardMessages.WM_SYSKEYUP))
                {
                    HandleNewInputEvent(new KeyboardEvent(key, KeyEventFlags.KeyUp, delay));
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
            if (code >= 0 && wParam != MouseMessages.WM_MOUSEMOVE)
            {
                HandleNewInputEvent(new MouseEvent(lParam.pt.x, lParam.pt.y, wParam, GetDelaySinceLastEvent()));
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

        private void HandleNewInputEvent(KeyboardEvent keyboardEvent)
        {
            if (!_reservedCommandInProgress)
            {
                _inputEventsBuffer.Add(keyboardEvent);
            }

            using (StreamWriter file = new StreamWriter(_logFileFullName, true))
            {
                file.WriteLine(keyboardEvent.ToString());
            }

            if (keyboardEvent.EventFlag == KeyEventFlags.KeyDown && !_reservedCommandInProgress && !_activeKeys.Contains(keyboardEvent.Key))
            {
                _activeKeys.Add(keyboardEvent.Key);
                CommandNames commandFound = _reservedCommandChecker.Check(_activeKeys);
                if (commandFound != CommandNames.None)
                {
                    _reservedCommandInProgress = true;
                    InvokeReservedCommandAction(commandFound);
                }
            }
            else if (keyboardEvent.EventFlag == KeyEventFlags.KeyUp && _activeKeys.Contains(keyboardEvent.Key))
            {
                _activeKeys.Remove(keyboardEvent.Key);
                if (!_activeKeys.Any() && _activeMouseButtons == 0)
                {
                    _reservedCommandInProgress = false;
                    SendInputEventsBufferToMediator();
                }
            }
        }

        private void HandleNewInputEvent(MouseEvent mouseEvent)
        {
            if (!_reservedCommandInProgress)
            {
                _inputEventsBuffer.Add(mouseEvent);
            }

            using (StreamWriter file = new StreamWriter(_logFileFullName, true))
            {
                file.WriteLine(mouseEvent.ToString());
            }

            if (!_reservedCommandInProgress && (mouseEvent.MouseMessage == MouseMessages.WM_LBUTTONDOWN || mouseEvent.MouseMessage == MouseMessages.WM_RBUTTONDOWN))
            {
                _activeMouseButtons++; ;
            }
            else if (mouseEvent.MouseMessage == MouseMessages.WM_LBUTTONUP || mouseEvent.MouseMessage == MouseMessages.WM_RBUTTONUP)
            {
                _activeMouseButtons--;
                if (!_activeKeys.Any() && _activeMouseButtons == 0)
                {
                    SendInputEventsBufferToMediator();
                }
            }
            else if (!_activeKeys.Any() && _activeMouseButtons == 0)
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

        private void InvokeReservedCommandAction(CommandNames commandFound)
        {
            switch(commandFound)
            {
                case CommandNames.Pause:
                    InvokeReservedCommandPause();
                    break;
                case CommandNames.Resume:
                    InvokeReservedCommandResume();
                    break;
                case CommandNames.Finish:
                    InvokeReservedCommandFinish();
                    break;
            }
        }

        private void InvokeReservedCommandPause()
        {
            _inputEventsBuffer = _reservedCommandChecker.RemoveReservedCommandEventsFromBuffer(_inputEventsBuffer, CommandNames.Pause);
            SendInputEventsBufferToMediator();
            _mediatorInstance.PauseRecording();
        }

        private void InvokeReservedCommandResume()
        {
            _inputEventsBuffer = _reservedCommandChecker.RemoveReservedCommandEventsFromBuffer(_inputEventsBuffer, CommandNames.Resume);
            SendInputEventsBufferToMediator();
            _mediatorInstance.ResumeRecording();
        }

        private void InvokeReservedCommandFinish()
        {
            _inputEventsBuffer = _reservedCommandChecker.RemoveReservedCommandEventsFromBuffer(_inputEventsBuffer, CommandNames.Finish);
            SendInputEventsBufferToMediator();
            // Since we will stop recording, we should update the boolean here
            _reservedCommandInProgress = false;
            _mediatorInstance.StopRecording();
        }
    }
}
